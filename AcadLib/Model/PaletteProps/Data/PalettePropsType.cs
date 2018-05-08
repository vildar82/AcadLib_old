using System.Collections.Generic;
using System.Linq;

namespace AcadLib.PaletteProps
{
    public class PalettePropsType
    {
        /// <summary>
        /// Название типа объектов
        /// </summary>
        public string Name { get; set; }

        public int Count { get; set; }
        public List<PalettePropsGroup> Groups { get; set; }
    }
}