using System.Windows.Controls;

namespace AcadLib.PaletteProps
{
    public class PalettePropVM
    {
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public Control ValueControl { get; set; }
    }
}