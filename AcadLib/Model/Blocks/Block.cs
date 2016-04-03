using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks
{
    public static class Block
    {
        public static Tolerance Tolerance01 = new Tolerance(0.01, 0.01);

        /// <summary>
        /// Копирование определенич блока из внешнего чертежа
        /// </summary>
        /// <param name="blName">Имя блока</param>
        /// <param name="fileDrawing">Полный путь к чертежу из которого копируется блок</param>
        /// <param name="destDb">База чертежа в который копируетсяя блок</param>
        /// <exception cref="Exception">Если нет блока в файле fileDrawing.</exception>
        public static ObjectId CopyBlockFromExternalDrawing(string blName, string fileDrawing, Database destDb,
                                                  DuplicateRecordCloning mode = DuplicateRecordCloning.Ignore)
        {
            ObjectId idRes;
            if (mode == DuplicateRecordCloning.Ignore)
            {
                using (var bt = destDb.BlockTableId.Open( OpenMode.ForRead) as BlockTable)
                {
                    if (bt.Has(blName))
                    {
                        return bt[blName];
                    }
                }
            }
            List<string> blNames = new List<string> { blName };
            var resCopy = CopyBlockFromExternalDrawing(blNames, fileDrawing, destDb, mode);
            if (!resCopy.TryGetValue(blName, out idRes))
            {
                throw new Autodesk.AutoCAD.Runtime.Exception(Autodesk.AutoCAD.Runtime.ErrorStatus.MissingBlockName, $"Не найден блок {blName}");
            }
            return idRes;
        }

        /// <summary>
        /// Копирование определенич блока из внешнего чертежа
        /// </summary>
        /// <param name="filter">Фильтр блоков, которые нужно копировать</param>
        /// <param name="fileDrawing">Полный путь к чертежу из которого копируется блок</param>
        /// <param name="destDb">База чертежа в который копируетсяя блок</param>
        /// <exception cref="Exception">Если нет блока в файле fileDrawing.</exception>
        /// <returns>Список пар значений имени блока и idBtr</returns>        
        public static Dictionary<string, ObjectId> CopyBlockFromExternalDrawing(Predicate<string> filter, string fileDrawing, Database destDb,
                                                  DuplicateRecordCloning mode = DuplicateRecordCloning.Ignore)
        {
            var resVal = new Dictionary<string, ObjectId>();
            using (var extDb = new Database(false, true))
            {
                extDb.ReadDwgFile(fileDrawing, System.IO.FileShare.ReadWrite, true, "");
                extDb.CloseInput(true);

                var valToCopy = new Dictionary<ObjectId, string>();

                using (var bt = (BlockTable)extDb.BlockTableId.Open(OpenMode.ForRead))
                {
                    foreach (var idBtr in bt)
                    {
                        using (var btr = idBtr.Open(OpenMode.ForRead) as BlockTableRecord)
                        {
                            if (!btr.IsLayout && !btr.IsDependent && !btr.IsAnonymous && filter(btr.Name))
                            {
                                valToCopy.Add(btr.Id, btr.Name);
                            }                            
                        }                        
                    }
                }
                // Копир
                if (valToCopy.Count > 0)
                {
                    // Получаем текущую базу чертежа
                    using (IdMapping map = new IdMapping())
                    {
                        using (var ids = new ObjectIdCollection(valToCopy.Keys.ToArray()))
                        {
                            destDb.WblockCloneObjects(ids, destDb.BlockTableId, map, mode, false);
                            foreach (var item in valToCopy)
                            {
                                resVal.Add(item.Value, map[item.Key].Value);
                            }
                        }
                    }
                }
            }
            return resVal;
        }

        /// <summary>
        /// Копирование определенич блока из внешнего чертежа
        /// </summary>
        /// <param name="blNames">Имена блоков</param>
        /// <param name="fileDrawing">Полный путь к чертежу из которого копируется блок</param>
        /// <param name="destDb">База чертежа в который копируетсяя блок</param>
        /// <exception cref="Exception">Если нет блока в файле fileDrawing.</exception>
        /// <returns>Список пар значений имени блока и idBtr</returns>        
        public static Dictionary<string, ObjectId> CopyBlockFromExternalDrawing(IList<string> blNames, string fileDrawing, Database destDb,
                                                DuplicateRecordCloning mode = DuplicateRecordCloning.Ignore)
        {
            var resVal = new Dictionary<string, ObjectId>();
            var uniqBlNames = blNames.Distinct(StringComparer.OrdinalIgnoreCase);

            using (var extDb = new Database(false, true))
            {
                extDb.ReadDwgFile(fileDrawing, System.IO.FileShare.ReadWrite, true, "");
                extDb.CloseInput(true);

                var valToCopy = new Dictionary<ObjectId, string>();

                using (var bt = (BlockTable)extDb.BlockTableId.Open(OpenMode.ForRead))
                {
                    foreach (var blName in uniqBlNames)
                    {
                        ObjectId id;
                        if (bt.Has(blName))
                        {
                            id = bt[blName];
                            valToCopy.Add(id, blName);
                        }
                    }
                }
                // Копир
                if (valToCopy.Count > 0)
                {
                    // Получаем текущую базу чертежа
                    using (IdMapping map = new IdMapping())
                    {
                        using (var ids = new ObjectIdCollection(valToCopy.Keys.ToArray()))
                        {
                            destDb.WblockCloneObjects(ids, destDb.BlockTableId, map, mode, false);
                            foreach (var item in valToCopy)
                            {
                                resVal.Add(item.Value, map[item.Key].Value);
                            }
                        }
                    }
                }
            }
            return resVal;
        }

        /// <summary>
        /// Копирование определения блока и добавление его в таблицу блоков.
        /// </summary>
        /// <param name="idBtrSource">Копируемый блок</param>
        /// <param name="name">Имя для нового блока</param>
        /// <returns>ID скопированного блока, или существующего в чертеже с таким именем.</returns>
        public static ObjectId CopyBtr(ObjectId idBtrSource, string name)
        {
            ObjectId idBtrCopy = ObjectId.Null;
            Database db = idBtrSource.Database;
            using (var t = db.TransactionManager.StartTransaction())
            {
                var btrSource = t.GetObject(idBtrSource, OpenMode.ForRead) as BlockTableRecord;
                var bt = t.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //проверка имени блока
                if (bt.Has(name))
                {
                    idBtrCopy = bt[name];
                }
                else
                {
                    var btrCopy = btrSource.Clone() as BlockTableRecord;
                    btrCopy.Name = name;
                    bt.UpgradeOpen();
                    idBtrCopy = bt.Add(btrCopy);
                    t.AddNewlyCreatedDBObject(btrCopy, true);
                    // Копирование объектов блока
                    ObjectIdCollection ids = new ObjectIdCollection();
                    foreach (ObjectId idEnt in btrSource)
                    {
                        ids.Add(idEnt);
                    }
                    IdMapping map = new IdMapping();
                    db.DeepCloneObjects(ids, idBtrCopy, map, false);
                }
                t.Commit();
            }
            return idBtrCopy;
        }

        /// <summary>
        /// Копирование листа
        /// </summary>
        /// <returns>ID Layout</returns>
        public static ObjectId CopyLayout(Database db, string layerSource, string layerCopy)
        {
            ObjectId idLayoutCopy = ObjectId.Null;
            Database dbOrig = HostApplicationServices.WorkingDatabase;
            HostApplicationServices.WorkingDatabase = db;
            LayoutManager lm = LayoutManager.Current;
            // Нужно проверить имена. Вдруг нет листа источника, или уже есть копируемый лист.
            lm.CopyLayout(layerSource, layerCopy);
            idLayoutCopy = lm.GetLayoutId(layerCopy);
            HostApplicationServices.WorkingDatabase = dbOrig;
            return idLayoutCopy;
        }

        /// <summary>
        /// Клонирование листа.
        /// Должна быть открыта транзакция!!!
        /// </summary>
        /// <param name="db">База в которой это производится. Должна быть WorkingDatabase</param>
        /// <param name="existLayoutName">Имя существующего листа, с которого будет клонироваться новый лист.
        /// Должен существовать в чертеже.</param>
        /// <param name="newLayoutName">Имя для нового листа.</param>
        /// <returns>ObjectId нового листа</returns>
        public static ObjectId CloneLayout(Database db, string existLayoutName, string newLayoutName)
        {
            ObjectId newLayoutId;
            ObjectId existLayoutId;
            using (WorkingDatabaseSwitcher sw = new WorkingDatabaseSwitcher(db))
            {
                LayoutManager lm = LayoutManager.Current;
                newLayoutId = lm.CreateLayout(newLayoutName);
                existLayoutId = lm.GetLayoutId(existLayoutName);
            }
            ObjectIdCollection objIdCol = new ObjectIdCollection();
            ObjectId idBtrNewLayout = ObjectId.Null;
            using (Layout newLayout = newLayoutId.GetObject(OpenMode.ForWrite) as Layout)
            {
                Layout curLayout = existLayoutId.GetObject(OpenMode.ForRead) as Layout;
                newLayout.CopyFrom(curLayout);
                idBtrNewLayout = newLayout.BlockTableRecordId;
                using (var btrCurLayout = curLayout.BlockTableRecordId.Open(OpenMode.ForRead) as BlockTableRecord)
                {
                    foreach (ObjectId objId in btrCurLayout)
                    {
                        objIdCol.Add(objId);
                    }
                }
            }
            IdMapping idMap = new IdMapping();
            db.DeepCloneObjects(objIdCol, idBtrNewLayout, idMap, false);
            return newLayoutId;
        }

        /// <summary>
        /// Получение валидной строки для имени блока. С замоной всех ненужных символов на .
        /// </summary>
        /// <param name="name">Имя для блока</param>
        /// <returns>Валидная строка имени</returns>
        public static string GetValidNameForBlock(string name)
        {
            return name.GetValidDbSymbolName();
            //string res = name;
            ////string testString = "<>/?\";:*|,='";
            //Regex pattern = new Regex("[<>/?\";:*|,=']");
            //res = pattern.Replace(name, ".");
            //res = res.Replace('\\', '.');
            //SymbolUtilityServices.ValidateSymbolName(res, false);
            //return res;
        }

        /// <summary>
        /// Проверка дублирования вхождений блоков
        /// </summary>
        /// <param name="blk1"></param>
        /// <param name="blk2"></param>
        /// <returns></returns>
        public static bool IsDuplicate(this BlockReference blk1, BlockReference blk2)
        {
            Tolerance tol = new Tolerance(1, 1);
            return
                blk1.OwnerId == blk2.OwnerId &&
                blk1.Name == blk2.Name &&
                Math.Round(blk1.Rotation, 1) == Math.Round(blk2.Rotation, 1) &&
                blk1.Position.IsEqualTo(blk2.Position, tol) &&
                blk1.ScaleFactors.IsEqualTo(blk2.ScaleFactors, tol);
        }

        /// <summary>
        /// Удаление всех объектов из блока.
        /// Блок должен быть открыт для записи
        /// </summary>
        /// <param name="btr"></param>
        public static void ClearEntity(this BlockTableRecord btr)
        {
            foreach (ObjectId idEnt in btr)
            {
                using (var ent = idEnt.Open(OpenMode.ForWrite) as Entity)
                {
                    ent.Erase();
                }
            }
        }

        /// <summary>
        /// Проверка натуральной трансформации блока - без масштабирования
        /// blRef.ScaleFactors.IsEqualTo(new Scale3d(1), Tolerance01)
        /// </summary>      
        public static bool CheckNaturalBlockTransform(this BlockReference blRef)
        {
            return blRef.ScaleFactors.IsEqualTo(new Scale3d(1), Tolerance01);
        }
    }
}