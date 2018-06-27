namespace AcadLib.Utils.Tabs.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using JetBrains.Annotations;
    using NetLib;
    using NetLib.WPF;
    using NetLib.WPF.Data;
    using ReactiveUI;

    public class TabsVM : BaseViewModel
    {
        public TabsVM([NotNull] IEnumerable<string> drawings)
        {
            Tabs = drawings.Select(GetTab).ToList();
            Ok = CreateCommand(OkExec);
            OpenFile = CreateCommand<TabVM>(OpenFileExec);
            this.WhenAnyValue(v => v.CheckAllTabs).Skip(1).Subscribe(s => Tabs.ForEach(t => t.Restore = s));
        }

        public List<TabVM> Tabs { get; set; }

        public ReactiveCommand Ok { get; set; }

        public bool IsOn { get; set; } = true;

        public ReactiveCommand OpenFile { get; set; }

        public bool CheckAllTabs { get; set; } = true;

        public override void OnPropertyChanged(string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(IsOn):
                    RestoreTabs.RestoreTabsIsOn(IsOn);
                    break;
            }

            base.OnPropertyChanged(propertyName);
        }

        private TabVM GetTab(string tab)
        {
            var fi = new FileInfo(tab);
            var tabVM = new TabVM
            {
                Name = Path.GetFileNameWithoutExtension(tab),
                File = tab,
                Restore = true,
                DateLastWrite = File.GetLastWriteTime(tab),
                Size = fi.Length,
                Image = NetLib.IO.Path.GetThumbnail(tab).ConvertToBitmapImage()
            };
            return tabVM;
        }

        private void OkExec()
        {
            DialogResult = true;
        }

        private void OpenFileExec(TabVM tab)
        {
            var argument = "/select, \"" + tab.File + "\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}
