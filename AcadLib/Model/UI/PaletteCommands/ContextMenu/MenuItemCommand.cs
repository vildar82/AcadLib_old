using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AcadLib.PaletteCommands
{
    /// <summary>
    /// Контекстное меню команды
    /// </summary>
    public class MenuItemCommand
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
        public ObservableCollection<MenuItemCommand> SubItems { get; set; }
    }
}
