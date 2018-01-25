using AcadLib.CommandLock.Data;
using AutoCAD_PIK_Manager.Settings;
using JetBrains.Annotations;
using NetLib;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace AcadLib.CommandLock
{
    public static class CommandLockService
    {
        private static FileData<CommandLocks> data;
        private static bool isInit;

        public static bool CanStartCommand([NotNull] string commandName)
        {
            if (!isInit && !Init()) return true;
            if (data.Data.Locks.TryGetValue(commandName, out var lc) && lc.IsActive)
            {
                if (lc.CanContinue)
                {
                    return MessageBox.Show($"Предупреждение:\n\n{lc.Message}\n\nПродолжить выполнение?",
                        "Блокировка", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes;
                }
                MessageBox.Show($"Команда заблокирована:\n\n{lc.Message}\n\nВыход", "Блокировка",
                    MessageBoxButton.OK, MessageBoxImage.Hand);
                return lc.CanContinue;
            }
            return true;
        }

        [NotNull]
        private static CommandLocks DefaultData()
        {
            return new CommandLocks
            {
                Locks = new CaseInsensitiveDictionary
                {
                    {
                        "CommandNameTest", new CommandLockInfo
                        {
                            IsActive = true,
                            CanContinue = true,
                            Message = "Ведутся технические работы."
                        }
                    }
                }
            };
        }

        private static bool Init()
        {
            try
            {
                var serverFolder = Path.Combine(PikSettings.ServerShareSettingsFolder,
                    @"AcadLib\CommandLock\CommandsLock.json");
                var localFile = IO.Path.GetUserPluginFile("CommandLock", "CommandsLock.json");
                data = new FileData<CommandLocks>(serverFolder, localFile, false);
                data.TryLoad();
                if (data.Data?.Locks?.Any() != true)
                {
                    Logger.Log.Error("CommandLockService - пустой список блокировок команд.");
                    data.Data = DefaultData();
                }
                var watcher = new FileWatcherRx(Path.GetDirectoryName(data.ServerFile), Path.GetFileName(data.ServerFile),
                    NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastAccess,
                    WatcherChangeTypes.All);
                watcher.Changed.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(s => data.TryLoad());
                watcher.Created.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(s => data.TryLoad());
                watcher.Renamed.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(s => data.TryLoad());
                watcher.Deleted.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(s =>
                {
                    data.Data = DefaultData();
                });
                isInit = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "CommandLockService.Init()");
            }
            return isInit;
        }
    }
}