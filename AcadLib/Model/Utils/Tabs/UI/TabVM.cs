namespace AcadLib.Utils.Tabs.UI
{
    using System;
    using System.Windows.Media;
    using NetLib.WPF;
    using NLog.LayoutRenderers;
    using ReactiveUI;

    public class TabVM : BaseModel
    {
        public string Name { get; set; }

        public string File { get; set; }

        public bool Restore { get; set; }

        public ImageSource Image { get; set; }

        public DateTime DateLastWrite { get; set; }

        public long Size { get; set; }
    }
}