using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib
{
    [Serializable]
    public class CommandInfo
    {
        /// <summary>
        /// Имя команды
        /// </summary>
        public string CommandName { get; set; }
        /// <summary>
        /// Список времени запуска
        /// </summary>
        public List<DateTime> DatesStart { get; set; }

        public CommandInfo() { }
        public CommandInfo(string name)
        {
            CommandName = name;
        }

        public void StartCommand()
        {
            if (DatesStart == null) DatesStart = new List<DateTime>();
            DatesStart.Add(DateTime.Now);
        }
    }
}
