using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AcadLib
{
    /// <summary>
    /// Загрузка вспомогательных сборок
    /// </summary>
    public static class LoadService
    {
        /// <summary>
        /// Загрузка сборки SpecBlocks.dll - для создания спецификация блоков, в соответствии с настройками.
        /// </summary>
        public static void LoadSpecBlocks()
        {
            LoadPackages("SpecBlocks.dll");
        }

        /// <summary>
        /// Morelinq
        /// </summary>
        public static void LoadMorelinq()
        {
            LoadPackages("MoreLinq.dll");
        }

        public static void LoadMicroMvvm ()
        {
            LoadPackages("MicroMvvm.dll");
            LoadPackages("System.Windows.Interactivity.dll");
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

        /// <summary>
        /// NetTopologySuite
        /// </summary>
        public static void LoadNetTopologySuite()
        {
            LoadPackages("NetTopologySuite.dll");
            LoadPackages("GeoAPI.dll");
            LoadPackages("PowerCollections.dll");            
        }        

        public static void LoadCatel()
        {
            LoadPackages(@"Catel\Catel.Core.dll");
            LoadPackages(@"Catel\Catel.MVVM.dll");
            LoadPackages(@"Catel\Catel.Extensions.Controls.dll");
            LoadPackages(@"Catel\Catel.Extensions.FluentValidation.dll");
            LoadPackages(@"Catel\Catel.Fody.Attributes.dll");
            LoadPackages(@"Catel\FluentValidation.dll");
            LoadPackages(@"Catel\System.Windows.Interactivity.dll");
        }

        public static void LoadMetro()
        {
            try
            {
                LoadPackages("System.Windows.Interactivity.dll");
            }
            catch
            {
            }
            LoadPackages(@"Metro\MahApps.Metro.dll");            
        }

        public static void LoadPackages(string name)
        {
            var dllServer = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder, @"packages\" + name);
            var dllLocal = Path.Combine(IO.Path.GetUserPluginFolder("packages"), name);
            CopyPackage(dllServer, dllLocal);
            LoadFrom(dllLocal);
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

        private static void CopyPackage(string dllServer, string dllLocal)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    var dllLocalDir = Path.GetDirectoryName(dllLocal);
                    if (!Directory.Exists(dllLocalDir))
                    {
                        Directory.CreateDirectory(dllLocalDir);
                    }
                    File.Copy(dllServer, dllLocal, true);
                }
                catch
                {
                }
            });
            task.Wait(2000);
        }
    }
}