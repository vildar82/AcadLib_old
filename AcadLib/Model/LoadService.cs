using System;
using System.IO;
using System.Reflection;

namespace AcadLib
{
    /// <summary>
    /// Загрузка вспомогательных сборок
    /// </summary>
    public static class LoadService
    {
        public static readonly string dllLocalPackages = IO.Path.GetUserPluginFolder("packages");

        public static void LoadScreenshotToSlack()
        {           
            
            LoadPackages("CloudinaryDotNet.dll");
            LoadPackages("ScreenshotToSlack.dll");            
        }

        /// <summary>
        /// Morelinq
        /// </summary>
        [Obsolete("Нафиг")]
        public static void LoadMorelinq()
        {
            LoadPackages("MoreLinq.dll");
        }
        /// <summary>
        /// EntityFramework
        /// </summary>
        public static void LoadEntityFramework()
        {            
            LoadFromTry(Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Dll\EntityFramework.dll"));
            LoadFromTry(Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Dll\EntityFramework.SqlServer.dll"));            
        }

        public static void LoadMDM()
        {
            LoadFromTry(Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\PIK_DB_Projects.dll"));                        
        }

        public static void LoadPackages(string name)
        {
            var dllLocal = Path.Combine(IO.Path.GetUserPluginFolder("packages"), name);
            LoadFromTry(dllLocal);
        }        

        public static void LoadFrom(string dll)
        {
            if (File.Exists(dll))
            {
                Assembly.LoadFrom(dll);
            }
            else
            {
                throw new Exception($"Не найден файл {dll}.");
            }
        }

        public static void LoadFromTry(string dll)
        {
            try
            {
                LoadFrom(dll);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "LoadFromTry - " + dll);
            }
        }

        public static void DeleteTry(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        public static void CopyPackagesLocal()
        {
	        try
	        {
		        var dllServer = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder, "packages");
		        NetLib.IO.Path.CopyDirectory(dllServer, dllLocalPackages);
	        }
	        catch (Exception ex)
	        {
		        Logger.Log.Error(ex, $"CopyPackagesLocal");
	        }
        }
    }
}