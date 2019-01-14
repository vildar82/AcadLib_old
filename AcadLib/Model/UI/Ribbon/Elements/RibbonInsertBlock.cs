namespace AcadLib.UI.Ribbon.Elements
{
    using System.Collections.Generic;
    using Blocks;

    public class RibbonInsertBlock : RibbonItemData
    {
        public string File { get; set; }
        public string BlockName { get; set; }
        public bool Explode { get; set; }
        public string Layer { get; set; }
        public List<Property> Properties { get; set; }
    }
}
