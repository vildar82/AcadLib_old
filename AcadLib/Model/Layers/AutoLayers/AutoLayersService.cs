using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Linq;
using System.Collections.Generic;
using AcadLib.Registry;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.Layers.AutoLayers
{
    public static class AutoLayersService
    {
        private static Document _doc;
        private static List<AutoLayer> autoLayers;
        private static AutoLayer curAutoLayer;
        private static List<ObjectId> idAddedEnts;
        private static bool IsStarted { get; set; }

        public static void Init()
        {
            try
            {
                Load();
                if (IsStarted) Start();
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "AutoLayersService Init");
            }
        }

        public static void Start()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            // Проверка - допустимости автослоев для группы пользователя
            if (!IsUserGroupAutoLayersAllowed() || string.IsNullOrEmpty(LayerExt.GroupLayerPrefix))
            {
                doc.Editor.WriteMessage($"\nАвтослои не поддерживаются для текущей группы пользователя - {AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup}");
                return;
            }
            Application.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
            Application.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;            
            autoLayers = GetAutoLayers();
            SubscribeDocument(doc);
            IsStarted = true;
            Save();
        }

        public static void Stop()
        {
            Application.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
            UnsubscribeDocument(_doc);
            _doc = null;
            curAutoLayer = null;            
            idAddedEnts = null;
            IsStarted = false;
            Save();
        }

        private static void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            SubscribeDocument(e.Document);
        }

        private static void SubscribeDocument(Document document)
        {
            // Отписка в старом документе
            UnsubscribeDocument(_doc);
            _doc = document;
            if (_doc == null) return;
            // Подписка на события команды нового документа
            _doc.CommandWillStart -= Doc_CommandWillStart;
            _doc.CommandWillStart += Doc_CommandWillStart;
        }
        private static void UnsubscribeDocument(Document document)
        {
            if (document == null) return;
            document.CommandWillStart -= Doc_CommandWillStart;
            document.CommandEnded -= Doc_CommandEnded;
            if (document.Database != null) document.Database.ObjectAppended -= Database_ObjectAppended;
            document.CommandCancelled -= Doc_CommandCancelled;
        }

        private static void Doc_CommandWillStart(object sender, CommandEventArgs e)
        {
            // Для команд автослоев - подписка на добавление объектов в чертеж
            curAutoLayer = GetAutoLayerCommand(e.GlobalCommandName);
            if (curAutoLayer == null) return;
            if (!(sender is Document document)) return;
            idAddedEnts = new List<ObjectId>();
            // Подписка на события добавления объектов и завершения команды
            document.Database.ObjectAppended -= Database_ObjectAppended;
            document.Database.ObjectAppended += Database_ObjectAppended;
            document.CommandEnded -= Doc_CommandEnded;
            document.CommandEnded += Doc_CommandEnded;
            // При Esc объекты все равно могут быть успешно добавлены в чертеж (если зациклена командв добавления размера, есть такие)
            document.CommandCancelled -= Doc_CommandCancelled;
            document.CommandCancelled += Doc_CommandCancelled;
        }

        private static void Doc_CommandEnded(object sender, CommandEventArgs e)
        {
            EndCommand(sender as Document, e.GlobalCommandName);
        }
        private static void Doc_CommandCancelled(object sender, CommandEventArgs e)
        {
            EndCommand(sender as Document, e.GlobalCommandName);            
        }

        private static void EndCommand(Document document, string globalCommandName)
        {
            curAutoLayer = GetAutoLayerCommand(globalCommandName);
            if (curAutoLayer == null || document == null) return;

            document.Database.ObjectAppended -= Database_ObjectAppended;
            document.CommandEnded -= Doc_CommandEnded;

            // Обработка объектов
            ProcessingAutoLayers(curAutoLayer, idAddedEnts);
            curAutoLayer = null;
        }

        private static void Database_ObjectAppended(object sender, ObjectEventArgs e)
        {
            // Сбор всех id добавляемых объектов            
            if (curAutoLayer != null)
            {
                idAddedEnts?.Add(e.DBObject.Id);
            }
        }

        private static List<AutoLayer> GetAutoLayers()
        {
            var als = new List<AutoLayer> {
                new AutoLayerDim()
            };
            return als;
        }

        private static AutoLayer GetAutoLayerCommand(string globalCommandName)
        {
            return autoLayers.Find(f=>f.IsAutoLayerCommand(globalCommandName));
        }

        private static void ProcessingAutoLayers(AutoLayer currentAutoLayerAutoLayer, List<ObjectId> idsAddedEnt)
        {
            var autoLayerEnts = currentAutoLayerAutoLayer.GetAutoLayerEnts(idsAddedEnt);
            if (autoLayerEnts == null) return;
            using (var t = _doc.TransactionManager.StartTransaction())
            {
                AutoLayerEntities(currentAutoLayerAutoLayer, autoLayerEnts);
                t.Commit();
            }
        }

        private static void AutoLayerEntities(AutoLayer autoLayer, IEnumerable<ObjectId> autoLayerEnts)
        {
            var layId = autoLayer.Layer.CheckLayerState();
            foreach (var idEnt in autoLayerEnts)
            {
                var ent = idEnt.GetObject<Entity>(OpenMode.ForWrite);
                if (ent != null && ent.LayerId != layId)
                {
                    ent.LayerId = layId;
                }
            }
        }

        /// <summary>
        /// Автослои для всех объектов чертежа
        /// </summary>
        public static void AutoLayersAll()
        {
            var document = Application.DocumentManager.MdiActiveDocument;            
            using (var t = document.TransactionManager.StartTransaction())
            {
                var db = document.Database;
                foreach (var idBtr in db.BlockTableId.GetObject<BlockTable>())
                {
                    var btr = idBtr.GetObject<BlockTableRecord>();
                    AutoLayersBtr(btr);
                }
                t.Commit();
            }
        }

        private static void AutoLayersBtr(BlockTableRecord btr)
        {
            if (btr == null) return;
            var idEnts = btr.Cast<ObjectId>().ToList();
            var idBlRefs = idEnts.Where(w=>w.ObjectClass == RXObject.GetClass(typeof(BlockReference)));
            idEnts = idEnts.Except(idBlRefs).ToList();
            foreach (var idBlRef in idBlRefs)
            {
                var blRef = idBlRef.GetObject<BlockReference>();                
                AutoLayersBtr(blRef.BlockTableRecord.GetObject<BlockTableRecord>());
            }
            foreach (var autoLayer in autoLayers)
            {
                var autoLayerEnts = autoLayer.GetAutoLayerEnts(idEnts);
                AutoLayerEntities(autoLayer, autoLayerEnts);
                idEnts = idEnts.Except(autoLayerEnts).ToList();
            }
        }        

        public static string GetInfo()
        {            
            var info = string.Empty;
            if (string.IsNullOrEmpty(LayerExt.GroupLayerPrefix))
            {
                info = $"\nТекущая группа пользователя '{AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup}' не поддерживает автослои.";
            }
            else
            {
                info += IsStarted ? "Автослои включены" : "Автослои выключены";
                if (!IsStarted || autoLayers == null) return info;
                info += Environment.NewLine;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var autoLayer in autoLayers)
                {
                    info += autoLayer.GetInfo() + Environment.NewLine;
                }
            }
            return info;
        }

        private static RegExt GetReg()
        {
            return new RegExt("AutoLayers");
        }

        private static void Load()
        {
            using (var reg = GetReg())
            {
                IsStarted = reg.Load("IsStarted", true);
            }
        }

        private static void Save()
        {
            using (var reg = GetReg())
            {
                reg.Save("IsStarted", IsStarted);
            }
        }

        private static bool IsUserGroupAutoLayersAllowed()
        {
            var userGroup = AutoCAD_PIK_Manager.Settings.PikSettings.UserGroupsCombined.First();
            return !userGroup.StartsWith(General.UserGroupGP) && 
                userGroup != General.UserGroupKRSBGK;
        }
    }
}