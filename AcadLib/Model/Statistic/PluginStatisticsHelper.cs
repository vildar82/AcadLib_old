using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Statistic.DataSetStatisticTableAdapters;

namespace AcadLib.Statistic
{
    public static class PluginStatisticsHelper
    {
        public static void PluginStart (CommandStart command)
        {
            Task.Run(() =>
            {
                if (General.IsCadManager()) return;
                try
                {
                    using (var pg = new C_PluginStatisticTableAdapter())
                    {
                        pg.Insert("AutoCAD", command.Plugin, command.CommandName,
                            FileVersionInfo.GetVersionInfo(command.Assembly.Location).ProductVersion,
                            command.Doc, Environment.UserName, DateTime.Now);
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
            var caller = new StackTrace().GetFrame(1).GetMethod();
            PluginStart(CommandStart.GetCallerCommand(caller));
        }
    }
}
