namespace AcadLib.UI.Ribbon.Editor.Data
{
    using System.Collections.ObjectModel;

    public class RibbonTabDataVM
    {
        public string Name { get; set; }
        public ObservableCollection<RibbonPanelDataVM> Panels { get; set; }
    }
}
