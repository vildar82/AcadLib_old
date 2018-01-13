using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using NetLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AcadLib
{
    /// <summary>
    /// Загрузка вспомогательных сборок
    /// </summary>
    [PublicAPI]
    public static class LoadService
    {
        public static readonly string dllLocalPackages = IO.Path.GetUserPluginFolder("packages");

        public static void CopyPackagesLocal()
        {
            try
            {
                var dllServer = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder, "packages");
                NetLib.IO.Path.CopyDirectory(dllServer, dllLocalPackages);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "CopyPackagesLocal");
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
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// EntityFramework
        /// </summary>
        public static void LoadEntityFramework()
        {
            LoadFromTry(Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Dll\EntityFramework.dll"));
            LoadFromTry(Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Dll\EntityFramework.SqlServer.dll"));
        }

        public static void LoadFrom([NotNull] string dll)
        {
            if (File.Exists(dll))
            {
                var asm = Assembly.LoadFrom(dll);
                Logger.Log.Info($"LoadFrom {asm.FullName}.");
            }
            else
            {
                throw new Exception($"Не найден файл {dll}.");
            }
        }

        /// <summary>
        /// Загрузка сборок из папки.
        /// </summary>
        public static void LoadFromFolder(string dir, SearchOption mode)
        {
            if (!Directory.Exists(dir)) return;
            var dlls = GetDllsForCurVerAcad(Directory.GetFiles(dir, "*.dll", mode).ToList());
            foreach (var dll in dlls)
            {
                LoadFromTry(dll);
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

        public static void LoadMDM()
        {
            LoadFromTry(Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\PIK_DB_Projects.dll"));
        }

        /// <summary>
        /// Morelinq
        /// </summary>
        [Obsolete("Нафиг")]
        public static void LoadMorelinq()
        {
            LoadPackages("MoreLinq.dll");
        }

        public static void LoadPackages([NotNull] string name)
        {
            var dllLocal = Path.Combine(IO.Path.GetUserPluginFolder("packages"), name);
            LoadFromTry(dllLocal);
        }

        public static void LoadScreenshotToSlack()
        {
            LoadPackages("CloudinaryDotNet.dll");
            LoadPackages("ScreenshotToSlack.dll");
        }

        [NotNull]
        private static List<string> GetDllsForCurVerAcad([NotNull] List<string> dlls)
        {
            var dllsToLoad = dlls.ToList();
            if (int.TryParse(HostApplicationServices.Current.releaseMarketVersion, out var ver))
            {
                foreach (var groupDllVer in dlls.SelectNulless(DllVer.GetDllVer).GroupBy(g => g.FileWoVer))
                {
                    var dllVers = groupDllVer.OrderByDescending(o => o.Ver).ToList();
                    var dllSimple = dlls.FirstOrDefault(f => f == groupDllVer.Key);
                    var dllWin = dllVers.FirstOrDefault(f => f.Ver <= ver);
                    if (dllWin == null && dllSimple == null)
                    {
                        dllWin = dllVers[0];
                    }
                    // Удалить лишние
                    if (dllWin != null)
                    {
                        dllVers.Remove(dllWin);
                        if (dllSimple != null)
                        {
                            dllsToLoad.Remove(dllSimple);
                        }
                    }
                    dllVers.ForEach(d => dllsToLoad.Remove(d.Dll));
                }
            }
            return dllsToLoad;
        }
    }

    [PublicAPI]
    internal class DllVer
    {
        public string Dll { get; set; }
        public string FileWoVer { get; set; }
        public int Ver { get; set; }

        public DllVer([NotNull] string fileDll, int ver)
        {
            Dll = fileDll;
            Ver = ver;
            FileWoVer = fileDll.Substring(0, fileDll.Length - 10) + ".dll";
        }

        [CanBeNull]
        public static DllVer GetDllVer([NotNull] string file)
        {
            DllVer dllVer = null;
            var match = Regex.Match(file, @"(_v(\d{4}).dll)$");
            if (match.Success && int.TryParse(match.Groups[2].Value, out var ver))
            {
                dllVer = new DllVer(file, ver);
            }
            return dllVer;
        }
    }
}