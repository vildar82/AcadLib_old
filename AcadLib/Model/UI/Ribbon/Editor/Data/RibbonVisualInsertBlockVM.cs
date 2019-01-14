using System.Collections.Generic;

namespace AcadLib.UI.Ribbon.Editor.Data
{
    using Elements;

    public class RibbonVisualInsertBlockVM : RibbonInsertBlockVM
    {
        public RibbonVisualInsertBlockVM(RibbonVisualInsertBlock item, List<BlockFile> blockFiles)
            : base(item, blockFiles)
        {
            Filter = item.Filter;
        }

        public string Filter { get; set; }

        public override RibbonItemData GetItem()
        {
            var item = new RibbonVisualInsertBlock();
            FillItem(item);
            item.Filter = Filter;
            return item;
        }
    }
}
