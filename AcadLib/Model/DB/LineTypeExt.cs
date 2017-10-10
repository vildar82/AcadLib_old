using System;
using System.IO;
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
            var id = db.GetLineTypeId(lineTypeName);
            if (!id.IsNull) return id;
            
            var file = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder,
                        "Support\\" + fileName);
            if (File.Exists(file))
            {
                try
                {
                    db.LoadLineTypeFile(lineTypeName, file);
                    return db.GetLineTypeId(lineTypeName);
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

        [Obsolete("Опечатка - используй GetLineTypeId")]
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

        public static ObjectId GetLineTypeId(this Database db, string lineTypeName)
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
