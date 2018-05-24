using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using AutoCAD_PIK_Manager;
using JetBrains.Annotations;
using NetLib;
using NetLib.Notification;

namespace AcadLib.Statistic
{
    public static class CheckUpdates
    {
        private static Timer timer;
        private static readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        internal static void Start()
        {
            timer = new Timer(o => CheckUpdatesNotify(), null, TimeSpan.FromSeconds(5), TimeSpan.FromHours(2));
        }

        /// <summary>
        /// Есть ли обновление настроек
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>True - есть новая версия</returns>
        public static bool Check([CanBeNull] out string msg)
        {
            var updateVersions = Update.GetVersions().Where(w=>w.UpdateRequired).ToList();
            if (updateVersions.Any())
            {
                var updates = updateVersions.JoinToString(v => $"{v.GroupName} от {v.VersionServerDate:dd.MM.yy HH:mm}", "\n");
                msg = $"Доступны обновления настроек:\n{updates}\nРекомендуется обновиться (перезапустить автокад).";
                return true;
            }
            msg = null;
            return false;
        }

        private static void CheckUpdatesNotify()
        {
            if (Check(out var msg))
            {
                dispatcher.Invoke(() =>
                {
                    try
                    {
                        Notify.ShowOnScreen(msg, NotifyType.Warning, new NotifyMessageOptions{ FontSize = 16});
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(ex, "CheckUpdatesNotify");
                    }
                });
            }
        }
    }
}
