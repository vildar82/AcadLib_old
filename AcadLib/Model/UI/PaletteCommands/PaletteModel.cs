using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MicroMvvm;

namespace AcadLib.PaletteCommands
{
    public class PaletteModel : ModelBase
    {
		protected PaletteModel()
        {

        }

        public PaletteModel(IEnumerable<IPaletteCommand> commands, string version)
        {
	        Version = version;
			PaletteCommands = new ObservableCollection<IPaletteCommand>();
            foreach (var item in commands)
            {
                if (item.Access == null || item.Access.Contains(Environment.UserName, StringComparer.OrdinalIgnoreCase))
                    PaletteCommands.Add(item);
            }
        }

        /// <summary>
        /// Цвет фона
        /// </summary>
        public System.Windows.Media.Brush Background { get; set; }
        /// <summary>
        /// Команды на палитре
        /// </summary>
        public ObservableCollection<IPaletteCommand> PaletteCommands { get; set; }

        public string Version { get; }
	}
}
