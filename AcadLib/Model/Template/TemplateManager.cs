// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 02 12 14:01

using System;
using System.IO;
using AcadLib.Layers;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using NetLib;
using System.Linq;
using AutoCAD_PIK_Manager.Settings;

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
            return new TemplateData { Layers = db.Layers().ToDictionary(k => k.Name) };
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
                Logger.Log.Warn(ex,$"Ошибка загрузки файла шаблона json '{file}'");
                return new TemplateData();
            }
        }

        [NotNull]
        public static string GetTemplateFolder(string templateFileName)
        {
            return Path.Combine(PikSettings.LocalSettingsFolder, $@"Template\{PikSettings.UserGroup}\{templateFileName}");
        }
    }
}