using AcadLib.Model.Statistic.DataSetStatisticTableAdapters;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Statistic
{
    [PublicAPI]
    public static class PluginStatisticsHelper
    {
        private static string _app;
        private static string _acadLibVer;
        private static string _acadYear;

        [NotNull]
        private static string App => _app ?? (_app = IsCivil() ? "Civil" : "AutoCAD");

        [NotNull]
        private static string AcadLibVer =>
            _acadLibVer ?? (_acadLibVer = Assembly.GetExecutingAssembly().GetName().Version.ToString());

        [NotNull]
        public static string AcadYear => _acadYear ?? (_acadYear = HostApplicationServices.Current.releaseMarketVersion);

        /// <summary>
        /// Запись статистики обновления настроек
        /// </summary>
        private static void UpdateSettings()
        {
            try
            {
                if (PikSettings.IsUpdatedSettings)
                    InsertStatistic($"{App} Update", "AcadLib", "Настройки последние", AcadLibVer, "");
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PluginStatisticsHelper.UpdateSettings");
            }
        }

        public static void AddStatistic()
        {
            try
            {
                var caller = new StackTrace().GetFrame(1).GetMethod();
                PluginStart(CommandStart.GetCallerCommand(caller));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PluginStatisticsHelper.AddStatistic");
            }
        }

        public static void PluginStart(CommandStart command)
        {
            if (!IsUserCanAddStatistic()) return;
            try
            {
                var version = command.Assembly != null
                    ? FileVersionInfo.GetVersionInfo(command.Assembly.Location).ProductVersion
                    : string.Empty;
                InsertStatistic(App, command.Plugin, command.CommandName, version, command.Doc);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PluginStart.");
            }
        }

        public static void StartAutoCAD()
        {
            if (!IsUserCanAddStatistic()) return;
            try
            {
                InsertStatistic($"{App} {AcadYear} Run", "AcadLib", $"{App} Run", AcadLibVer, "");
                // Статистика обновления настроек
                UpdateSettings();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "StartAutoCAD.");
            }
        }

        private static bool IsUserCanAddStatistic()
        {
            return !General.IsCadManager() && !General.IsBimUser;
        }

        private static void InsertStatistic(string appName, string plugin, string command, string version, string doc)
        {
            Task.Run(() =>
            {
                try
                {
                    using (var pg = new C_PluginStatisticTableAdapter())
                    {
                        pg.Insert(appName, plugin, command, version,
                            doc, Environment.UserName, DateTime.Now, null);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, "PluginStatisticsHelper Insert.");
                }
            });
        }

        private static bool IsCivil()
        {
            try { return CivilTest.IsCivil(); } catch { return false;}
        }
    }
}