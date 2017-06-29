using AcadLib.Model.Statistic.DataSetStatisticTableAdapters;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace AcadLib.Statistic
{
    public static class PluginStatisticsHelper
    {
        public static void StartAutoCAD()
        {
            Task.Run(() =>
            {
                try
                {
                    if (!General.IsCadManager())
                    {
                        var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        using (var pg = new C_PluginStatisticTableAdapter())
                        {
                            pg.Insert("AutoCAD Run", "AcadLib", "AutoCAD Run", version,
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

        public static void PluginStart (CommandStart command)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!General.IsCadManager())
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

        private static bool IsCivilAssembly(Assembly assm)
        {
            return assm?.GetName().Name.StartsWith("PIK_GP") == true;
        }

        public static void AddStatistic ()
        {
            try
            {
                var caller = new StackTrace().GetFrame(1).GetMethod();
                PluginStart(CommandStart.GetCallerCommand(caller));
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex, "PluginStatisticsHelper.AddStatistic");
            }
        }
    }
}
