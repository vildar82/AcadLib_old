using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AcadLib.PaletteCommands
{
    [PublicAPI]
    public interface IPaletteCommand
    {
        List<string> Access { get; }
        ICommand Command { get; set; }
        List<MenuItemCommand> ContexMenuItems { get; set; }
        string Description { get; }
        string Group { get; }
        string HelpMedia { get; }
        ImageSource Image { get; }
        bool IsTest { get; }
        string Name { get; }

        void Execute();
    }
}