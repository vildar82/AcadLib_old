using AcadLib.Model.Statistic.DataSetStatisticTableAdapters;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;
using NetLib;
using NetLib.Notification;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.Statistic
{
    [PublicAPI]
    public static class PluginStatisticsHelper
    {
        private static string _app;
        private static string _acadLibVer;
        private static string _acadYear;
        private static bool? _isCivil;

        [NotNull]
        private static string App => _app ?? (_app = IsCivil ? "Civil" : "AutoCAD");

        [NotNull]
        public static string AcadYear => _acadYear ?? (_acadYear = HostApplicationServices.Current.releaseMarketVersion);

        public static bool IsCivil => (bool) (_isCivil ?? (_isCivil = GetIsCivil()));

        /// <summary>
        /// Запись статистики обновления настроек
        /// </summary>
        private static void UpdateSettings()
        {
            try
            {
                if (PikSettings.IsUpdatedSettings)
                    InsertStatistic($"{App} Update", "AcadLib", "Настройки последние", Commands.AcadLibVersion.ToString(), "");
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
                if (command.Plugin.IsNullOrEmpty()) command.Plugin = command.Assembly?.GetName().Name;
                if (command.Doc.IsNullOrEmpty()) command.Doc = Application.DocumentManager.MdiActiveDocument?.Name;
                InsertStatistic(App, command.Plugin, command.CommandName, version, command.Doc);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PluginStart.");
            }
        }

        public static void StartAutoCAD()
        {
            try
            {
                InsertStatistic($"{App} {AcadYear} Run", "AcadLib", $"{App} Run", Commands.AcadLibVersion.ToString(), "");
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
                        pg.Insert(appName, plugin ?? "", command ?? "", version ?? "",
                            doc ?? "", Environment.UserName, DateTime.Now, null);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, "PluginStatisticsHelper Insert.");
                }
            });
        }

        private static bool GetIsCivil()
        {
            try { return CivilTest.IsCivil(); } catch { return false;}
        }

        private static void CheckCommandUpdate([NotNull] CommandInfo command)
        {
            var task = Task.Run(() =>
            {
                var localAsmFile = "";
                var serverAsmFile = "";
                if (!NetLib.IO.Path.IsEqualsDataDir(localAsmFile, serverAsmFile))
                {
                    return $"Доступна новая версия плагина '' от " +
                           $"{File.GetLastWriteTime(serverAsmFile):dd.MM.yy HH:mm}." +
                           "\nРекомендуется обновиться.";
                }
                return null;
            });
            if (task.Wait(300) && task.Result != null)
            {
                try
                {
                    Notify.ShowScreenNotify(task.Result, NotifyType.Warning, new NotifyMessageOptions{ FontSize = 14});
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}