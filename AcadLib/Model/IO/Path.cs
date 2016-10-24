using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.IO
{
    public static class Path
    {
        /// <summary>
        /// Получение файла в общей папке настроек на сервере \\dsk2.picompany.ru\project\CAD_Settings\AutoCAD_server\ShareSettings\[UserGroup]\pluginName\fileName
        /// </summary>
        /// <param name="pluginName">Имя плагина (команды)</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Полный путь к файлу. Наличие файла не проверяется. Папка создается</returns>
        public static string GetSharedFile (string pluginName, string fileName)
        {
            string resFilePath = string.Empty;
            var pluginFolder = System.IO.Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder,
                AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup, pluginName);
            if (!Directory.Exists(pluginFolder))
            {
                try
                {
                    Directory.CreateDirectory(pluginFolder);
                }
                catch { }
            }
            resFilePath = System.IO.Path.Combine(pluginFolder, fileName);
            return resFilePath;
        }

        /// <summary>
        /// Создает папку в темпе и возрвращает полный путь
        /// </summary>        
        public static string GetTemporaryDirectory ()
        {
            string tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
