using System.Collections.Generic;

namespace AcadLib.UI.Ribbon.Options
{
    /// <summary>
    /// Настройки ленты
    /// </summary>
    public class RibbonOptions
    {
        public List<ItemOptions> Tabs { get; set; } = new List<ItemOptions>();
    }
}
