// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 02 13 15:03

using System;
using System.IO;
using System.Linq;
using AcadLib.Layers;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using NetLib;

namespace AcadLib.Template
{
    /// <summary>
    ///     Управление шаблонами
    /// </summary>
    [PublicAPI]
    public static class TemplateManager
    {
        public static void ExportToJson(this TemplateData tData, [NotNull] string file)
        {
            tData.Serialize(file);
        }

        [NotNull]
        public static TemplateData LoadFromDb([NotNull] Database db)
        {
            return new TemplateData {Layers = db.Layers().ToDictionary(k => k.Name)};
        }

        public static TemplateData LoadFromJson(string file)
        {
            if (!File.Exists(file))
            {
                Logger.Log.Warn($"Не найден файл шаблона json - {file}");
                return new TemplateData();
            }
            try
            {
                return file.Deserialize<TemplateData>();
            }
            catch (Exception ex)
            {
                Logger.Log.Warn(ex, $"Ошибка загрузки файла шаблона json '{file}'");
                return new TemplateData();
            }
        }

        /// <summary>
        ///     Полный путь к шаблону (из папки Template настроек)
        /// </summary>
        /// <param name="templateFileName">Имя файла шаблона с расширением</param>
        /// <returns></returns>
        [NotNull]
        public static string GetTemplateFile(string templateFileName)
        {
            return Path.Combine(PikSettings.LocalSettingsFolder, $@"Template\{PikSettings.UserGroup}\{templateFileName}");
        }
    }
}