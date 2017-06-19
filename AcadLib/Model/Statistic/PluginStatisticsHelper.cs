using AcadLib.Model.Statistic.DataSetStatisticTableAdapters;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AcadLib.Statistic
{
    public static class PluginStatisticsHelper
    {
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
                            pg.Insert("AutoCAD", command.Plugin, command.CommandName, version,
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
