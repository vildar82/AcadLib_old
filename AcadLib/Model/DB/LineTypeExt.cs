namespace AcadLib
{
    using System;
    using System.IO;
    using Autodesk.AutoCAD.DatabaseServices;
    using JetBrains.Annotations;

    [PublicAPI]
    public static class LineTypeExt
    {
        [Obsolete("Опечатка - используй GetLineTypeId")]
        public static ObjectId GetLayerId([NotNull] this Database db, string lineTypeName)
        {
            using (var lt = (LinetypeTable)db.LinetypeTableId.Open(OpenMode.ForRead))
            {
                return lt.Has(lineTypeName) ? lt[lineTypeName] : ObjectId.Null;
            }
        }

        public static ObjectId GetLineTypeId([NotNull] this Database db, string lineTypeName)
        {
            using (var lt = (LinetypeTable)db.LinetypeTableId.Open(OpenMode.ForRead))
            {
                return lt.Has(lineTypeName) ? lt[lineTypeName] : ObjectId.Null;
            }
        }

        /// <summary>
        /// Загрузка типа линии из файла поддержи lin в папке Support PIK
        /// Если файл не найден или тип линии, то вернется текущий тип линии чертежа
        /// </summary>
        /// <param name="db"></param>
        /// <param name="lineTypeName">тип линии</param>
        /// <param name="fileName">Имя файла</param>
        public static ObjectId LoadLineTypePIK(
            [NotNull] this Database db,
            string lineTypeName,
            string fileName = "GOST 2.303-68.lin")
        {
            var id = db.GetLineTypeId(lineTypeName);
            if (!id.IsNull)
                return id;

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
    }
}