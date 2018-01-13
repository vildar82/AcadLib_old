using JetBrains.Annotations;
using System;
using System.IO;

namespace AcadLib.IO
{
    [PublicAPI]
    public static class Path
    {
        /// <summary>
        /// Копирование папки - рекурсивно. Папка создается.
        /// </summary>
        /// <param name="source">Откуда</param>
        /// <param name="target">Куда</param>
        [Obsolete]
        public static void CopyDirTo([NotNull] this DirectoryInfo source, [NotNull] DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (var fi in source.GetFiles())
            {
                Console.WriteLine($@"Copying {target.FullName}\{fi.Name}");
                fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
            }
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirTo(diSourceSubDir, nextTargetSubDir);
            }
        }

        [Obsolete]
        public static void CopyDirTo([NotNull] string sourceDir, [NotNull] string targetDir)
        {
            CopyDirTo(new DirectoryInfo(sourceDir), new DirectoryInfo(targetDir));
        }

        /// <summary>
        /// Получение файла в общей папке настроек на сервере \\dsk2.picompany.ru\project\CAD_Settings\AutoCAD_server\ShareSettings\[UserGroup]\pluginName\fileName
        /// </summary>
        /// <param name="pluginName">Имя плагина (команды)</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Полный путь к файлу. Наличие файла не проверяется. Папка создается</returns>
        [NotNull]
        public static string GetSharedFile([NotNull] string pluginName, [NotNull] string fileName)
        {
            return System.IO.Path.Combine(GetSharedPluginFolder(pluginName), fileName);
        }

        [NotNull]
        public static string GetSharedPluginFolder([NotNull] string pluginName)
        {
            var pluginFolder = System.IO.Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder,
                AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup, pluginName);
            if (!Directory.Exists(pluginFolder))
            {
                try
                {
                    Directory.CreateDirectory(pluginFolder);
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"GetSharedpluginFolder - pluginName={pluginName}");
                }
            }
            return pluginFolder;
        }

        /// <summary>
        /// Создает папку в темпе и возрвращает полный путь
        /// </summary>
        [NotNull]
        [Obsolete]
        public static string GetTemporaryDirectory()
        {
            var tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        /// <summary>
        /// Пользовательская папка настроек
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static string GetUserPikFolder()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
            var pikFolder = AutoCAD_PIK_Manager.CompanyInfo.NameEngShort;
            var pikAppDataFolder = System.IO.Path.Combine(appData, pikFolder, "AutoCAD");
            if (!Directory.Exists(pikAppDataFolder))
            {
                Directory.CreateDirectory(pikAppDataFolder);
            }
            return pikAppDataFolder;
        }

        /// <summary>
        /// Путь к пользовательскому файлу настроек плагина
        /// </summary>
        /// <param name="plugin">Имя плагина</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Полный путь к файлу</returns>
        [NotNull]
        public static string GetUserPluginFile([NotNull] string plugin, [NotNull] string fileName)
        {
            var pluginFolder = GetUserPluginFolder(plugin);
            return System.IO.Path.Combine(pluginFolder, fileName);
        }

        /// <summary>
        /// Путь к папке плагина
        /// </summary>
        /// <param name="plugin">Имя плагина - имя папки</param>
        /// <returns>Полный путь</returns>
        [NotNull]
        public static string GetUserPluginFolder([NotNull] string plugin)
        {
            var pikFolder = GetUserPikFolder();
            var pluginFolder = System.IO.Path.Combine(pikFolder, plugin);
            if (!Directory.Exists(pluginFolder))
                Directory.CreateDirectory(pluginFolder);
            return pluginFolder;
        }
    }
}