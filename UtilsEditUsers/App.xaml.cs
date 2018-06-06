using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AcadLib;
using AcadLib.User;
using AutoCAD_PIK_Manager.Settings;
using NetLib.AD;
using NLog;
using StringExt = NetLib.StringExt;

namespace UtilsEditUsers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ILogger Log { get; } = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                var groups = ADUtils.GetCurrentUserADGroups(out var fio);
                General.IsBimUser = groups.Any(g => StringExt.EqualsIgnoreCase(g, "010583_Отдел разработки и автоматизации") ||
                                                    StringExt.EqualsIgnoreCase(g, "010596_Отдел внедрения ВIM") ||
                                                    StringExt.EqualsIgnoreCase(g, "010576_УИТ"));
                PikSettings.ServerShareSettingsFolder = @"\\picompany.ru\pikp\lib\_CadSettings\AutoCAD_server\ShareSettings\";
                UserSettingsService.UsersEditor();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}