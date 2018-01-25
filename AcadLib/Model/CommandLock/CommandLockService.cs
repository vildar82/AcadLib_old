using System;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using AcadLib.CommandLock.Data;
using AcadLib.Files;
using JetBrains.Annotations;
using NetLib;

namespace AcadLib.CommandLock
{
    public static class CommandLockService
    {
        private static readonly FileData<CommandLocks> data;

        static CommandLockService()
        {
            data = FileDataExt.GetSharedFileData<CommandLocks>("CommandLock", "CommandsLock", false);
            data.TryLoad();
            var watcher = new FileWatcherRx(Path.GetDirectoryName(data.ServerFile), Path.GetFileName(data.ServerFile),
                NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastAccess,
                WatcherChangeTypes.All);
            watcher.Changed.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(s => data.TryLoad());
            watcher.Created.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(s => data.TryLoad());
            watcher.Renamed.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(s => data.TryLoad());
            watcher.Deleted.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(s =>
            {
                data.Data = DefaultData();
                data.TrySave();
            });
        }

        public static bool CanStartCommand([NotNull] string commandName)
        {
            if (data.Data.Locks.TryGetValue(commandName, out var lc) && lc.IsActive)
            {
                if (lc.CanContinue)
                {
                    return MessageBox.Show($"Команда заблокирована:\n\n{lc.Message}\n\nПродолжить выполнение?",
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
    }
}