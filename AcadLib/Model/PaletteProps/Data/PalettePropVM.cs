using System.Windows.Controls;

namespace AcadLib.PaletteProps
{
    /// <summary>
    /// Свойство для палитры
    /// </summary>
    public class PalettePropVM
    {
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public Control ValueControl { get; set; }
    }
}