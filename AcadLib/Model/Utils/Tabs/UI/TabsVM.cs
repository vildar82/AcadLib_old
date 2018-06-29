namespace AcadLib.Utils.Tabs.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using History.Db;
    using JetBrains.Annotations;
    using NetLib.WPF;
    using ReactiveUI;

    public class TabsVM : BaseViewModel
    {
        public TabsVM([NotNull] IEnumerable<string> drawings)
        {
            Tabs = drawings.Select(s => GetTab(s, true)).ToList();
            Ok = CreateCommand(OkExec);
            OpenFile = CreateCommand<TabVM>(OpenFileExec);
            this.WhenAnyValue(v => v.CheckAllTabs).Skip(1).Subscribe(s => Tabs.ForEach(t => t.Restore = s));
            LoadHistory();
        }

        public List<TabVM> Tabs { get; set; }
        public ReactiveList<TabVM> History { get; set; } = new ReactiveList<TabVM>();

        public ReactiveCommand Ok { get; set; }

        public bool IsOn { get; set; } = true;

        public ReactiveCommand OpenFile { get; set; }

        public bool CheckAllTabs { get; set; } = true;

        public bool HasHistory { get; set; }

        public double RestoreTabsColRestoreWidth { get; set; } = 300;

        public double RestoreTabsColNameWidth { get; set; } = 500;

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

        private TabVM GetTab(string tab, bool restore)
        {
            return new TabVM(tab, restore);
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

        private void LoadHistory()
        {
            Task.Run(() =>
            {
                var tabs = new HashSet<string>();
                var dbHistory = new DbHistory();
                var files = dbHistory.LoadHistoryFiles();
                foreach (var item in files)
                {
                    if (tabs.Add(item.DocPath))
                    {
                        dispatcher.Invoke(() => History.Add(GetTab(item.DocPath, false)));
                    }
                }
            });
        }
    }
}
