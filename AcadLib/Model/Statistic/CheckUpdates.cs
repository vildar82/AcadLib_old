﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using AutoCAD_PIK_Manager;
using AutoCAD_PIK_Manager.Settings;
using JetBrains.Annotations;
using NetLib;
using NetLib.Notification;

namespace AcadLib.Statistic
{
    public static class CheckUpdates
    {
        private static Timer timer;
        private static readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private static List<string> serverFilesVer;
        private static List<FileWatcherRx> watchers;
        private static readonly Subject<bool> changes = new Subject<bool>();
        /// <summary>
        /// Отключенные уведомления групп пользователем
        /// </summary>
        private static readonly Dictionary<string, DateTime> notNotifyGroups = new Dictionary<string, DateTime>();

        internal static void Start()
        {
            timer = new Timer(o => CheckUpdatesNotify(true), null, TimeSpan.FromSeconds(5), TimeSpan.FromHours(2));
            changes.Throttle(TimeSpan.FromMilliseconds(1000)).Subscribe(s => CheckUpdatesNotify(true));
        }

        /// <summary>
        /// Есть ли обновление настроек
        /// </summary>
        /// <param name="includeUserNotNotify">включая отключенные пользователем обновления групп</param>
        /// <param name="msg"></param>
        /// <param name="updateVersions">Обновленные группы настроек</param>
        /// <returns>True - есть новая версия</returns>
        public static bool Check(bool includeUserNotNotify, [CanBeNull] out string msg, [CanBeNull] out List<GroupInfo> updateVersions)
        {
            try
            {
                var versions = Update.GetVersions();
                SubscribeChanges(versions);
                updateVersions = Update.GetVersions().Where(w => w.UpdateRequired && NeedNotify(w.UpdateDescription) &&
                                                                 (!includeUserNotNotify || !IsNotNotify(w))).ToList();
                if (updateVersions.Any())
                {
                    var updates = updateVersions.JoinToString(v => 
                            $"{v.GroupName} от {v.VersionServerDate:dd.MM.yy HH:mm}" +
                            $"{(v.UpdateDescription.IsNullOrEmpty() ? "" : $" - '{v.UpdateDescription}'")}","\n");
                    msg = $"Доступны обновления настроек:\n{updates}\nРекомендуется обновиться (перезапустить автокад).";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "CheckUpdates.Check");
            }
            msg = null;
            updateVersions = null;
            return false;
        }

        private static bool IsNotNotify([NotNull] GroupInfo groupInfo)
        {
            if (notNotifyGroups.TryGetValue(groupInfo.GroupName, out var updateDate))
            {
                return updateDate <= groupInfo.VersionServerDate;
            }
            return false;
        }

        private static void SubscribeChanges(IEnumerable<GroupInfo> versions)
        {
            if (serverFilesVer == null)
            {
                serverFilesVer = versions.Select(s => s.VersionServerFile).ToList();
                watchers = new List<FileWatcherRx>();
                foreach (var file in serverFilesVer)
                {
                    var watcher = new FileWatcherRx(Path.GetDirectoryName(file), Path.GetFileName(file));
                    watcher.Changed.Throttle(TimeSpan.FromMilliseconds(200))
                        .Subscribe(OnFileVersionChanged);
                }
            }
        }

        private static bool NeedNotify([CanBeNull] string updateDesc)
        {
            return updateDesc == null || !(updateDesc.StartsWith("no", StringComparison.OrdinalIgnoreCase) ||
                     updateDesc.StartsWith("нет", StringComparison.OrdinalIgnoreCase));
        }

        private static void OnFileVersionChanged([NotNull] EventPattern<FileSystemEventArgs> e)
        {
            try
            {
                Debug.WriteLine($"{e.EventArgs.FullPath}|{e.EventArgs.ChangeType}, {e.Sender}");
                var desc = File.ReadLines(e.EventArgs.FullPath, Encoding.Default).Skip(1).FirstOrDefault();
                if (NeedNotify(desc)) changes.OnNext(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex,"CheckUpdates OnFileVersionChanged");
            }
        }

        public static void CheckUpdatesNotify(bool includeUserNotNotify)
        {
            if (Check(includeUserNotNotify,out var msg, out var updateVersions))
            {
                dispatcher.Invoke(() =>
                {
                    try
                    {
                        Notify.ShowOnScreen(msg, NotifyType.Warning, new NotifyMessageOptions
                        {
                            FontSize = 16,
                            NotificationClickAction = () =>
                            {
                                if (updateVersions?.Any() == true)
                                {
                                    foreach (var updateVersion in updateVersions)
                                    {
                                        notNotifyGroups[updateVersion.GroupName] = updateVersion.VersionServerDate;
                                    }
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(ex, "CheckUpdatesNotify");
                    }
                });
            }
            else
            {
                "Нет обновлений настроек на сервере.".WriteToCommandLine();
            }
        }
    }
}
