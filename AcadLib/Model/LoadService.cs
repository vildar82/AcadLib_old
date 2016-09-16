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
        }

        /// <summary>
        /// EntityFramework
        /// </summary>
        public static void LoadEntityFramework()
        {
            LoadPackages("EntityFramework.dll");
            LoadPackages("EntityFramework.SqlServer.dll");
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

        private static void LoadPackages(string name)
        {
            var dll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\packages\" + name);
            if (File.Exists(dll))
            {
                Assembly.LoadFrom(dll);
            }
            else
            {
                throw new Exception($"Не найден файл {dll}.");
            }
        }
    }
}