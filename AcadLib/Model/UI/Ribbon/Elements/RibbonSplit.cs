namespace AcadLib.UI.Ribbon.Elements
{
    using System.Collections.Generic;

    /// <summary>
    /// Выпадающий список элементов
    /// </summary>
    public class RibbonSplit : RibbonItemData
    {
        public List<RibbonItemData> Items { get; set; }
    }
}
