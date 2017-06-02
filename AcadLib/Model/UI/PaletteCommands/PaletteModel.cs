using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MicroMvvm;

namespace AcadLib.PaletteCommands
{
    public class PaletteModel : ModelBase
    {
	    ObservableCollection<IPaletteCommand> _paletteCommands;
	    System.Windows.Media.Brush _background;

		protected PaletteModel()
        {

        }

        public PaletteModel(IEnumerable<IPaletteCommand> commands, string version)
        {
	        Version = version;
			_paletteCommands = new ObservableCollection<IPaletteCommand>();
            foreach (var item in commands)
            {
                if (item.Access == null || item.Access.Contains(Environment.UserName, StringComparer.OrdinalIgnoreCase))
                    _paletteCommands.Add(item);
            }
        }        

        /// <summary>
        /// Цвет фона
        /// </summary>
        public System.Windows.Media.Brush Background {
            get => _background;
	        set { _background = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Команды на палитре
        /// </summary>
        public ObservableCollection<IPaletteCommand> PaletteCommands {
            get => _paletteCommands;
	        set { _paletteCommands = value; RaisePropertyChanged(); }
        }

	    public string Version { get; }
	}
}
