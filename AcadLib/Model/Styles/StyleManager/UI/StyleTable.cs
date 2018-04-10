using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using AcadLib.Errors;
using AcadLib.Filer;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;
using NetLib.WPF;
using NLog;
using ReactiveUI;
using ReactiveUI.Legacy;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace AcadLib.Styles.StyleManager.UI
{
    public class StyleTable : BaseModel
    {
        private static ILogger Log { get; } = LogManager.GetCurrentClassLogger();

        public StyleTable(StyleManagerVM baseVM) : base(baseVM)
        {
            Delete = CreateCommand<Style>(DeleteExec);
        }

        public string Name { get; set; }
        public ReactiveCommand Delete{ get; set; }
        public ObjectId StyleTableId { get; set; }

        [CanBeNull]
        public ReactiveList<Style> Styles { get; set; }

        private void DeleteExec([NotNull] Style style)
        {
            var doc = AcadHelper.Doc;
            var db = doc.Database;
            if (style.Id.Database != db)
            {
                throw new Exception($"Переключись на чертеж '{db.Filename}'");
            }
            bool needTwice = false;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                var dbo = style.Id.GetObjectT<DBObject>(OpenMode.ForWrite);
                if (dbo is BlockTableRecord btr)
                {
                    foreach (var dbObject in btr.GetBlockReferenceIds(true, false).GetObjects<DBObject>(OpenMode.ForWrite))
                    {
                        var ownerName = dbObject.OwnerId.GetObject<BlockTableRecord>()?.Name;
                        Inspector.AddError("Удалено", $"Удалено вхождение блока из {ownerName}");
                        dbObject.Erase();
                    }
                    if (btr.IsDynamicBlock)
                    {
                        foreach (var anonymBtr in btr.GetAnonymousBlockIds().GetObjects<BlockTableRecord>())
                        {
                            foreach (var dbObject in anonymBtr.GetBlockReferenceIds(true, false).GetObjects<DBObject>(OpenMode.ForWrite))
                            {
                                var ownerName = dbObject.OwnerId.GetObject<BlockTableRecord>()?.Name;
                                Inspector.AddError("Удалено", $"Удалено вхождение анонимного блока из {ownerName}");
                                dbObject.Erase();
                            }
                        }
                    }
                    btr.Erase();
                }
                else
                {
                    try
                    {
                        var replaceId = ObjectId.Null;
                        var replaceName = "";
                        var table = StyleTableId.GetObjectT<DBObject>();
                        switch (table)
                        {
                            case SymbolTable st:
                                if (st is LayerTable lt)
                                {
                                    if (style.Name == "0") throw new Exception("Нельзя удалить 0 слой");
                                    replaceId = lt["0"];
                                    replaceName = "0";
                                }
                                else if (style.Name != "ПИК" && st.Has("ПИК"))
                                {
                                    replaceId = st["ПИК"];
                                    replaceName = "ПИК";
                                }
                                else if (style.Name != "Standart" && st.Has("Standart"))
                                {
                                    replaceId = st["Standart"];
                                    replaceName = "Standart";
                                }
                                else
                                {
                                    replaceId = st.Cast<ObjectId>().FirstOrDefault(s => s != style.Id);
                                    replaceName = replaceId.GetObject<SymbolTableRecord>()?.Name;
                                }
                                break;
                            case DBDictionary dict:
                                if (dict.Contains("ПИК"))
                                {
                                    if (dict["ПИК"] is DictionaryEntry entry)
                                    {
                                        replaceId = (ObjectId) entry.Value;
                                        replaceName = "ПИК";
                                    }
                                }
                                else if (dict.Contains("Standart"))
                                {
                                    if (dict["Standart"] is DictionaryEntry entry)
                                    {
                                        replaceId = (ObjectId) entry.Value;
                                        replaceName = "Standart";
                                    }
                                }
                                else
                                {
                                    var entry = dict.Cast<DictionaryEntry>()
                                        .FirstOrDefault(e => (string) e.Key != style.Name);
                                    replaceId = (ObjectId) entry.Value;
                                    replaceName = entry.Key.ToString();
                                }
                                break;
                        }
                        if (dbo is LayerTableRecord)
                        {
                            ReplaceLayer(db, style.Id, replaceId, replaceName);
                        }
                        // Найти все ссылки и зменить
                        var refs = dbo.GetReferences();
                        refs.HardPointerIds.ForEach(p => ReplacePointer(p, style.Id, replaceId, replaceName));
                        needTwice = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Ошибка замены референсов в стиле '{style.Name}' в таблице стилей '{Name}'");
                    }
                }
                t.Commit();
            }
            if (needTwice)
            {
                using (doc.LockDocument())
                using (var t = doc.TransactionManager.StartTransaction())
                {
                    var dbo = style.Id.GetObjectT<DBObject>(OpenMode.ForWrite);
                    dbo.Erase();
                    t.Commit();
                }
            }
            Styles?.Remove(style);
            Inspector.Show();
        }

        private void ReplaceLayer([NotNull] Database db, ObjectId layerId, ObjectId replaceId, string replaceName)
        {
            var bt = db.BlockTableId.GetObjectT<BlockTable>();
            foreach (var btr in bt.GetObjects<BlockTableRecord>())
            {
                foreach (var entity in btr.GetObjects<Entity>(OpenMode.ForRead).Where(w=>w.LayerId == layerId))
                {
                    var entW = entity.Id.GetObjectT<Entity>(OpenMode.ForWrite);
                    entW.LayerId = replaceId;
                    Inspector.AddError($"Заменен слой объекта '{entW.Id.ObjectClass.Name}' на '{replaceName}'");
                }
            }
        }

        private void ReplacePointer(ObjectId pointerId, ObjectId styleId, ObjectId replaceId, string replaceName)
        {
            var dbo = pointerId.GetObjectT<DBObject>(OpenMode.ForWrite);
            if (dbo is BlockTableRecord btr)
            {
                foreach (var objectId in btr)
                {
                    ReplacePointer(objectId, styleId, replaceId, replaceName);
                }
            }
            var props = dbo.GetType().GetProperties();
            var prop = props.FirstOrDefault(p =>
            {
                try
                {
                    var val = p.GetValue(dbo);
                    if (val is ObjectId valId)
                    {
                        return valId == styleId;
                    }
                }
                catch (Exception ex)
                {
                    Log.Info($"p.GetValue(dbo) prop={p.Name}, {dbo} - {ex.Message}");
                }
                return false;
            });
            if (prop == null)
            {
                Log.Info($"prop == null {dbo}");
            }
            else
            {
                try
                {
                    prop.SetValue(dbo, replaceId);
                    var msg = $"Заменена ссылка в объекте '{dbo.Id.ObjectClass.Name}' на '{replaceName}'";
                    if (dbo is Entity ent)
                    {
                        Inspector.AddError(msg, ent);
                    }
                    else
                    {
                        Inspector.AddError(msg);
                    }
                    Log.Info($"prop.SetValue prop={prop.Name} - {dbo.Id.ObjectClass.Name}");
                }
                catch (Exception ex)
                {
                    Log.Info($"prop.SetValue prop={prop.Name}, {ex.Message} - {dbo.Id.ObjectClass.Name}");
                }
            }
        }
    }
}