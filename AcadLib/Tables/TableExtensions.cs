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
               idStyle = copyTableStyleFromTemplate(db);
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

      // Копирование стиля таблиц ПИК из файла шаблона
      private static ObjectId copyTableStyleFromTemplate(Database db)
      {
         ObjectId idStyleDest = ObjectId.Null;
         // файл шаблона
         string fileTemplate = Path.Combine(PikSettings.LocalSettingsFolder, "Template", PikSettings.UserGroup, PikSettings.UserGroup + ".dwg");
         if (File.Exists(fileTemplate))
         {
            using (Database dbTemplate = new Database (false, true))
            {
               dbTemplate.ReadDwgFile(fileTemplate, FileOpenMode.OpenForReadAndAllShare, false, "");
               var idStyleInTemplate = getTableStylePik(dbTemplate);
               if(!idStyleInTemplate.IsNull)
               {
                  IdMapping map = new IdMapping();
                  db.WblockCloneObjects(new ObjectIdCollection(new ObjectId[] { idStyleInTemplate }),
                                db.TableStyleDictionaryId, map, DuplicateRecordCloning.Ignore, false);
                  idStyleDest = map[idStyleInTemplate].Value;
               }
            }
         }
         return idStyleDest;
      }
   }
}
