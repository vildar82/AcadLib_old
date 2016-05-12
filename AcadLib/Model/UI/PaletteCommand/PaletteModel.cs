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

namespace AcadLib.PaletteCommands
{
    public class PaletteModel : INotifyPropertyChanged
    {
        private System.Windows.Media.Brush _background;
        public System.Windows.Media.Brush Background
        {
            get
            {
                return _background;
            }
            set
            {
                if (value != _background)
                {
                    _background = value;
                    NotifyPropertyChanged("Background");
                }
            }
        }
        private ObservableCollection<IPaletteCommand> _paletteCommands;
        public ObservableCollection<IPaletteCommand> PaletteCommands
        {
            get
            {
                return _paletteCommands;
            }
            set
            {
                if (value != _paletteCommands)
                {
                    _paletteCommands = value;
                    NotifyPropertyChanged("PaletteCommands");
                }
            }
        }

        public PaletteModel(IEnumerable<IPaletteCommand> commands)
        {
            _paletteCommands = new ObservableCollection<IPaletteCommand>();
            foreach (var item in commands)            
                _paletteCommands.Add(item);                        
        }

        public event PropertyChangedEventHandler PropertyChanged;        
        protected void NotifyPropertyChanged(string info)
        {            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));            
        }        
    }
}
