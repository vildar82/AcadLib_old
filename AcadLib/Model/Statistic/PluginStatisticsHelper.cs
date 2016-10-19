using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Model.Statistic.DataSetStatisticTableAdapters;

namespace AcadLib.Model.Statistic
{
    public static class PluginStatisticsHelper
    {
        public static void PluginStart (CommandStart command)
        {
            if (General.IsCadManager()) return;
            try
            {
                var pg = new C_PluginStatisticTableAdapter();
                pg.Insert("AutoCAD", command.Plugin, command.CommandName,
                    FileVersionInfo.GetVersionInfo(command.Assembly.Location).ProductVersion,
                    command.Doc, Environment.UserName, DateTime.Now);
            }            
            catch(Exception ex) {
                Logger.Log.Error(ex, "PluginStatisticsHelper.PluginStart");                
            }
        }
    }
}
