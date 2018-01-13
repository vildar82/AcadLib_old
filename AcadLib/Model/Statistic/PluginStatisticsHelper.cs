using AcadLib.Model.Statistic.DataSetStatisticTableAdapters;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace AcadLib.Statistic
{
    [PublicAPI]
    public static class PluginStatisticsHelper
    {
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
            Task.Run(() =>
            {
                try
                {
                    if (!General.IsCadManager() && !General.IsBimUser)
                    {
                        var version = command.Assembly != null
                            ? FileVersionInfo.GetVersionInfo(command.Assembly.Location).ProductVersion
                            : string.Empty;
                        using (var pg = new C_PluginStatisticTableAdapter())
                        {
                            var app = IsCivilAssembly(command.Assembly) ? "Civil" : "AutoCAD";
                            pg.Insert(app, command.Plugin, command.CommandName, version,
                                command.Doc, Environment.UserName, DateTime.Now, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, "PluginStatisticsHelper.PluginStart");
                }
            });
        }

        public static void StartAutoCAD()
        {
            Task.Run(() =>
            {
                try
                {
                    if (!General.IsCadManager() && !General.IsBimUser)
                    {
                        var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        using (var pg = new C_PluginStatisticTableAdapter())
                        {
                            var appRun = IsCivilGroup() ? "Civil Run" : "AutoCAD Run";
                            pg.Insert(appRun, "AcadLib", appRun, version,
                                "", Environment.UserName, DateTime.Now, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, "PluginStatisticsHelper.StartAutoCAD");
                }
            });
        }

        private static bool IsCivilAssembly([CanBeNull] Assembly assm)
        {
            return assm?.GetName().Name?.Contains("Civil") == true;
        }

        private static bool IsCivilGroup()
        {
            return AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup.StartsWith("ГП") ||
                   AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup == "НС";
        }
    }
}