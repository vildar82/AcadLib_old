using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autodesk.AutoCAD.DatabaseServices
{
   public static  class TableExtensions
   {
      /// <summary>
      /// Получение табличного стиля ПИК
      /// </summary>      
      public static ObjectId GetTableStylePIK(this Database db)
      {
         ObjectId idStyle = getTableStylePik(db);
         if (idStyle.IsNull)
         {
            // Копирование стиля таблиц из шаблона
            try
            {
               idStyle = copyObjectFromTemplate(db, getTableStylePik, db.TableStyleDictionaryId);
            }
            catch
            { }
            if (idStyle.IsNull)
            {
               idStyle = db.Tablestyle;
            }
         }
         return idStyle;
      }

      private static ObjectId getTableStylePik(Database db)
      {
         ObjectId idStyle = ObjectId.Null;
         using (var dictTableStyles = db.TableStyleDictionaryId.Open(OpenMode.ForRead) as DBDictionary)
         {
            if (dictTableStyles.Contains("ПИК"))
            {
               idStyle = dictTableStyles.GetAt("ПИК");
            }
         }
         return idStyle;
      }

      private static ObjectId getTextStylePik(Database db)
      {
         ObjectId idStyle = ObjectId.Null;
         using (var textStyles = db.TextStyleTableId.Open(OpenMode.ForRead) as TextStyleTable)
         {
            if (textStyles.Has("PIK"))
            {
               idStyle = textStyles["PIK"];
            }
         }
         return idStyle;
      }

      private static ObjectId GetTextStylePIK(this Database db)
      {
         ObjectId idStyle = getTextStylePik(db);         

         if (idStyle.IsNull)
         {
            // Копирование стиля таблиц из шаблона
            try
            {
               idStyle = copyObjectFromTemplate(db, getTextStylePik, db.TextStyleTableId);
            }
            catch
            { }
            if (idStyle.IsNull)
            {
               idStyle = db.Tablestyle;
            }
         }
         return idStyle;
      }

      // Копирование стиля таблиц ПИК из файла шаблона
      private static ObjectId copyObjectFromTemplate(Database db, Func<Database, ObjectId> getObjectId, ObjectId ownerIdTable)
      {
         ObjectId idStyleDest = ObjectId.Null;
         // файл шаблона
         string fileTemplate = Path.Combine(PikSettings.LocalSettingsFolder, "Template", PikSettings.UserGroup, PikSettings.UserGroup + ".dwg");
         if (File.Exists(fileTemplate))
         {
            using (Database dbTemplate = new Database (false, true))
            {
               dbTemplate.ReadDwgFile(fileTemplate, FileOpenMode.OpenForReadAndAllShare, false, "");
               ObjectId idStyleInTemplate = getObjectId(dbTemplate);               
               if (!idStyleInTemplate.IsNull)
               {
                  IdMapping map = new IdMapping();
                  db.WblockCloneObjects(new ObjectIdCollection(new ObjectId[] { idStyleInTemplate }),
                                ownerIdTable, map, DuplicateRecordCloning.Ignore, false);
                  idStyleDest = map[idStyleInTemplate].Value;
               }
            }
         }
         return idStyleDest;
      }
   }
}
