namespace AcadLib.Utils.Tabs.UI
{
    using NetLib.WPF;

    public class TabVM : BaseModel
    {
        public string Name { get; set; }

        public string File { get; set; }

        public bool Restore { get; set; }
    }
}