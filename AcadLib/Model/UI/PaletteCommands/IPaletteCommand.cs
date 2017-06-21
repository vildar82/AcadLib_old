using System.Collections.Generic;
using System.Windows.Media;

namespace AcadLib.PaletteCommands
{
    public interface IPaletteCommand
    {
        string HelpMedia { get; }
        ImageSource Image { get; }
        string Name { get; }
        string Description { get; }
        string Group { get; }
        List<string> Access { get; }
        List<MenuItemCommand> ContexMenuItems { get; set; }
        bool IsTest { get; }
        void Execute();
    }
}
