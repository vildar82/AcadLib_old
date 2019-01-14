namespace AcadLib.UI.Ribbon.Editor.Data
{
    using Elements;

    public class RibbonBreakVM : RibbonItemDataVM
    {
        public RibbonBreakVM(RibbonBreak item)
            : base(item)
        {
        }

        public override RibbonItemData GetItem()
        {
            var item = new RibbonBreak();
            return item;
        }
    }
}
