using System;
using System.Text.RegularExpressions;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks
{
   public static class Block
   {
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
         ObjectId idCopyedBtr = ObjectId.Null;
         using (var extDb = new Database(false, true))
         {
            extDb.ReadDwgFile(fileDrawing, System.IO.FileShare.ReadWrite, true, "");
            extDb.CloseInput(true);
            ObjectIdCollection ids = new ObjectIdCollection();

            using (var bt = (BlockTable)extDb.BlockTableId.Open(OpenMode.ForRead))
            {
               if (bt.Has(blName))
               {
                  ids.Add(bt[blName]);
               }
               else
               {
                  throw new Exception(string.Format("Не найдено определение блока {0} в файле {1}", blName, fileDrawing));
               }
            } 
            // Если нашли – добавим блок
            if (ids.Count != 0)
            {
               // Получаем текущую базу чертежа
               IdMapping map = new IdMapping();
               destDb.WblockCloneObjects(ids, destDb.BlockTableId, map, mode, false);
               idCopyedBtr = map[ids[0]].Value;
            }
         }
         return idCopyedBtr;
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
         LayoutManager lm = LayoutManager.Current;
         ObjectId newLayoutId = lm.CreateLayout(newLayoutName);
         ObjectId existLayoutId = lm.GetLayoutId(existLayoutName);
         Layout newLayout = newLayoutId.GetObject(OpenMode.ForWrite) as Layout;
         Layout curLayout = existLayoutId.GetObject(OpenMode.ForRead) as Layout;
         newLayout.CopyFrom(curLayout);
         ObjectIdCollection objIdCol = new ObjectIdCollection();
         using (var btrCurLayout = curLayout.BlockTableRecordId.Open(OpenMode.ForRead) as BlockTableRecord)
         {
            foreach (ObjectId objId in btrCurLayout)
            {
               objIdCol.Add(objId);
            }
         }
         IdMapping idMap = new IdMapping();
         db.DeepCloneObjects(objIdCol, newLayout.BlockTableRecordId, idMap, false);
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
         Tolerance tol = new Tolerance(1,1);
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
   }
}