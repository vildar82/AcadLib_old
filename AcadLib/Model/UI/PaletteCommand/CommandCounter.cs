using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AcadLib.Files;
using AutoCAD_PIK_Manager;

namespace AcadLib
{
    /// <summary>
    /// Счетчик команд. Имена команд и время их запуска для данного пользователя.
    /// </summary>
    [Serializable]
    public class CommandCounter
    {
        /// <summary>
        /// Хранит счетчик комманд. Доступен всем.
        /// </summary>        
        public static CommandCounter Counter { get; set; }

        /// <summary>
        /// Файл xml для хранения счетчика команд.
        /// </summary>
        public static string FileXml { get; set; }

        /// <summary>
        /// Список команд.
        /// </summary>
        public List<CommandInfo> Commands { get; set; }

        public DateTime LastClear { get; set; }

        /// <summary>
        /// Словарь команд - имя команды и счетчик.
        /// </summary>        
        private Dictionary<string, CommandInfo> _commands;  

        public static void CountCommand(string name)
        {
            if (Counter == null) Init();
            Counter.Add(name);
        }

        private void Add(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (_commands == null) _commands = new Dictionary<string, CommandInfo>();
            CommandInfo ci;
            if (!_commands.TryGetValue(name, out ci))
            {
                ci = new CommandInfo(name);
                _commands.Add(name, ci);
            }
            ci.StartCommand();
        }

        public static void Init()
        {            
            // загрузка из файла
            FileXml =General.GetUserDataFile("","CommandCounter.xml");
            if (File.Exists(FileXml))
            {
                SerializerXml xmlSer = new SerializerXml(FileXml);
                try
                {
                    Counter = xmlSer.DeserializeXmlFile<CommandCounter>();
                    if (Counter != null)
                    {
                        Counter._commands = Counter.Commands.Where(c=>!string.IsNullOrEmpty(c.CommandName)).ToDictionary(c=>c.CommandName, c=>c);
                        Logger.Log.Debug($"Counter.Commands.Count={Counter.Commands.Count}");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"Не удалось десериализовать файл {FileXml}");
                }
            }
            Logger.Log.Debug($"Counter = new CommandCounter();");
            Counter = new CommandCounter();                        
        }

        public void Save()
        {
            if (_commands == null) return;            
            Commands = _commands.Values.ToList();
            // Удаление старых вызовов команд - старше 2 месяцев
            removeOldDates(Commands);
            try
            {
                if (!File.Exists(FileXml))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FileXml));
                }                
                SerializerXml xmlSer = new SerializerXml(FileXml);
                xmlSer.SerializeList(this);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"Не удалось сериализовать CommandCounter в {FileXml}");
            }
        }

        private void removeOldDates(List<CommandInfo> list)
        {
            // Выполнение очистки один раз в 30 дней
            var delta30 = TimeSpan.FromDays(30);
            var now = DateTime.Now;
            if (now - LastClear > delta30)
            {
                //Удаление старых вызовов комманд - старше 60 дней
                var delta60 = TimeSpan.FromDays(60);
                list.ForEach(c => c.DatesStart.RemoveAll(d => (now-d) > delta60));
                LastClear = now;
            }
        }
    }
}
