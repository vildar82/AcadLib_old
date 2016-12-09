using MicroMvvm;
using NetLib.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AcadLib.PaletteCommands
{
    /// <summary>
    /// Контекстное меню команды
    /// </summary>
    public class MenuItemCommand : ViewModelBase
    {
        public MenuItemCommand(string name, ICommand command)
        {
            Name = name;
            Command = command;            
        }
        
        public string Name { get; set; }
        public ICommand Command { get; set; }        
        public object CommandParameter { get; set; }
        public object Icon { get; set; }
        public ObservableCollection<MenuItemCommand> SubItems { get; set;}
    }
}
