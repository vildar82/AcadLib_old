using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class LineTypeExt
    {
        /// <summary>
        /// Загрузка типа линии из файла поддержи lin в папке Support PIK
        /// Если файл не найден или тип линии, то вернется текущий тип линии чертежа
        /// </summary>
        /// <param name="db"></param>
        public static ObjectId LoadLineTypePIK(this Database db, string lineTypeName, string fileName = "GOST 2.303-68.lin")
        {
            var id = db.GetLayerId(lineTypeName);
            if (!id.IsNull) return id;
            
            string file = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder,
                        "Support\\" + fileName);
            if (File.Exists(file))
            {
                try
                {
                    db.LoadLineTypeFile(lineTypeName, file);
                    return db.GetLayerId(lineTypeName);
                }
                catch
                {
                    Logger.Log.Error($"Ошибка загрузки типа линии - LoadLineTypePIK '{lineTypeName}'");
                }
            }
            else
            {
                Logger.Log.Error($"Не найден файл типов линий '{file}'");
            }
            return db.Celtype;
        }

        public static ObjectId GetLayerId(this Database db, string lineTypeName)
        {
            using (var lt = db.LinetypeTableId.Open(OpenMode.ForRead) as LinetypeTable)
            {
                if (lt.Has(lineTypeName))
                {
                    return lt[lineTypeName];
                }
                return ObjectId.Null;
            }
        }
    }
}
