using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NetLib;

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

        [NotNull]
        public static List<DllVer> GetDllsForCurVerAcad([NotNull] List<string> dlls)
        {
            Logger.Log.Info($"GetDllsForCurVerAcad dlls={dlls.JoinToString(Path.GetFileNameWithoutExtension)}");
            var dllsToLoad = new List<DllVer>();
            if (int.TryParse(HostApplicationServices.Current.releaseMarketVersion, out var ver))
            {
                var dllVerGroups = dlls.Select(DllVer.GetDllVer).GroupBy(g => g.FileWoVer).ToList();
                foreach (var groupDllVer in dllVerGroups)
                {
                    if (groupDllVer.Skip(1).Any())
                    {
                        var dllWin = groupDllVer.FirstOrDefault(f => f.Ver == ver) ??
                                     groupDllVer.OrderByDescending(o => o.Ver).FirstOrDefault(d => d.Ver <= ver);
                        if (dllWin == null) continue; // Могут быть только специфичные версии, не для текущей - типа Acad_SheetSet_v2018 (нет для 2015)
                        dllsToLoad.Add(dllWin);
                    }
                    else
                    {
                        dllsToLoad.Add(groupDllVer.First());
                    }
                }
            }
            Logger.Log.Info($"GetDllsForCurVerAcad dllsToLoad={dllsToLoad.JoinToString(s=> Path.GetFileNameWithoutExtension(s.Dll))}");
            return dllsToLoad;
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
            try
            {
                if (!Directory.Exists(dir)) return;
                var dlls = GetDllsForCurVerAcad(Directory.GetFiles(dir, "*.dll", mode).ToList());
                foreach (var dll in dlls)
                {
                    LoadFromTry(dll.Dll);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"LoadFromFolder {dir}");
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
    }
}