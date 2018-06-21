namespace AcadLib.PaletteProps
{
    using System.Collections.Generic;
    using System.Linq;

    public class PalettePropsType
    {
        /// <summary>
        /// Название типа объектов
        /// </summary>
        public string Name { get; set; }

        public int Count => Groups?.Sum(s => s.Count) ?? 0;

        public List<PalettePropsGroup> Groups { get; set; }
    }
}