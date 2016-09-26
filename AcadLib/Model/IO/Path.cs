using System;
using System.Collections.Generic;
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
            var pluginFolder = System.IO.Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder,
                AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup, pluginName);
            if (!System.IO.Directory.Exists(pluginFolder))
            {
                System.IO.Directory.CreateDirectory(pluginFolder);
            }
            var file = System.IO.Path.Combine(pluginFolder, fileName);
            return file;
        }
    }
}
