using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.AutoCAD.Runtime;
using NetLib.WPF;
using MicroMvvm;

namespace AcadLib.PaletteCommands
{
    public class PaletteModel : ModelBase
    {           
        public PaletteModel()
        {

        }
        public PaletteModel(IEnumerable<IPaletteCommand> commands)
        {
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
            get { return _background; }
            set { _background = value; RaisePropertyChanged(); }
        }
        System.Windows.Media.Brush _background;

        /// <summary>
        /// Команды на палитре
        /// </summary>
        public ObservableCollection<IPaletteCommand> PaletteCommands {
            get { return _paletteCommands; }
            set { _paletteCommands = value; RaisePropertyChanged(); }
        }
        ObservableCollection<IPaletteCommand> _paletteCommands;       
    }
}
