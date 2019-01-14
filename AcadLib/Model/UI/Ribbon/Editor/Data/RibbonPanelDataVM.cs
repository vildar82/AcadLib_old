namespace AcadLib.UI.Ribbon.Editor.Data
{
    using System.Collections.ObjectModel;
    using Elements;

    public class RibbonPanelDataVM
    {
        public string Name { get; set; }
        public ObservableCollection<RibbonItemDataVM> Items { get; set; }
    }
}