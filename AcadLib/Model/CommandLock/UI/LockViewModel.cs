using AcadLib.CommandLock.Data;
using JetBrains.Annotations;
using NetLib.WPF;
using System.Collections.Generic;

namespace AcadLib.CommandLock.UI
{
    public class LockViewModel : BaseViewModel
    {
        public List<Button> Buttons { get; set; }

        public CommandLockInfo Command { get; }
        public string ImageKey { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }

        public LockViewModel([NotNull] CommandLockInfo command)
        {
            Command = command;
            Message = command.Message;
            Buttons = new List<Button>();
            if (command.CanContinue)
            {
                ImageKey = "../../../Resources/notify.png";
                Title = "Предупреждение";
                Buttons.Add(new Button { Name = "Продолжить", Command = CreateCommand(() => DialogResult = true) });
            }
            else
            {
                ImageKey = "../../../Resources/stop.png";
                Title = "Команда заблокирована";
            }
            Buttons.Add(new Button { Name = "Выход", Command = CreateCommand(() => DialogResult = false) });
        }
    }
}