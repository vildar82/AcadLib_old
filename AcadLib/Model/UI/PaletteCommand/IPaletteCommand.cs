using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AcadLib.PaletteCommands
{
    public interface IPaletteCommand
    {
        ImageSource Image { get; }
        string Name { get; }
        string Description { get; }
        void Execute();
    }
}
