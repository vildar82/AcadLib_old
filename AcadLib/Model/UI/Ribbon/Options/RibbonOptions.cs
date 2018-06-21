namespace AcadLib.UI.Ribbon.Options
{
    using System.Collections.Generic;

    /// <summary>
    /// Настройки ленты
    /// </summary>
    public class RibbonOptions
    {
        public List<ItemOptions> Tabs { get; set; } = new List<ItemOptions>();
    }
}