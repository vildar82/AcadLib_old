namespace AcadLib.UI.Ribbon.Editor.Data
{
    using Elements;

    public class RibbonCommandVM : RibbonItemDataVM
    {
        public RibbonCommandVM(RibbonCommand item)
            : base(item)
        {
            Command = item.Command;
        }

        public string Command { get; set; }

        public override RibbonItemData GetItem()
        {
            var item = new RibbonCommand();
            FillItem(item);
            item.Command = Command;
            return item;
        }
    }
}
