namespace AcadLib.Utils.Tabs.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using History;
    using JetBrains.Annotations;
    using Model.Utils.Tabs.History.Db;
    using NetLib;
    using NetLib.Notification;
    using NetLib.WPF;
    using ReactiveUI;
    using Tabs.History.Db;

    public class TabsVM : BaseViewModel
    {
        private ReactiveList<TabVM> history = new ReactiveList<TabVM>();

        public TabsVM([NotNull] IEnumerable<string> drawings, bool isOn)
        {
            IsOn = isOn;
            Tabs = drawings.Select(s => GetTab(s, true, DateTime.MinValue)).ToList();
            Ok = CreateCommand(OkExec);
            this.WhenAnyValue(v => v.CheckAllTabs).Skip(1).Subscribe(s => Tabs.ForEach(t => t.Restore = s));
            HasRestoreTabs = Tabs.Count > 0;
            if (!HasRestoreTabs)
            {
                HasHistory = true;
            }

            this.WhenAnyValue(v => v.HistorySearch).Skip(1).Subscribe(s => History.Reset());
            History = history.CreateDerivedCollection(t => t, HistoryFilter, HistoryOrder);
            LoadHistory();
        }

        public List<TabVM> Tabs { get; set; }

        public ReactiveCommand Ok { get; set; }

        public bool IsOn { get; set; }

        public bool CheckAllTabs { get; set; } = true;

        public bool HasHistory { get; set; }
        public bool HasRestoreTabs { get; set; }

        public double RestoreTabsColRestoreWidth { get; set; } = 300;

        public double RestoreTabsColNameWidth { get; set; } = 500;

        public string HistorySearch { get; set; }

        public IReactiveDerivedList<TabVM> History { get; set; }

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

        private TabVM GetTab(string tab, bool restore, DateTime start)
        {
            return new TabVM(tab, restore)
            {
                Start = start
            };
        }

        private void OkExec()
        {
            DialogResult = true;
        }

        public void OpenFileExec(TabVM tab)
        {
            if (File.Exists(tab.File))
            {
                var argument = "/select, \"" + tab.File + "\"";
                Process.Start("explorer.exe", argument);
            }
            else
            {
                var notify = new Notify(new NotifyOptions(TimeSpan.FromSeconds(2),
                    Window,
                    NotifyCorner.BottomCenter,
                    with: 300,
                    offsetX: 0,
                    offsetY: 5));
                notify.Show("Путь скопирован в буфер обмена",
                    NotifyType.Information,
                    new NotifyMessageOptions { ShowCloseButton = false });
                Clipboard.SetText(tab.File);
            }
        }

        private bool HistoryFilter(TabVM tab)
        {
            return HistorySearch.IsNullOrEmpty() || Regex.IsMatch(tab.File, Regex.Escape(HistorySearch), RegexOptions.IgnoreCase);
        }

        private int HistoryOrder(TabVM t1, TabVM t2)
        {
            return t2.Start.CompareTo(t1.Start);
        }

        private void LoadHistory()
        {
            var cacheHistory = HistoryModel.LoadHistoryCache();
            foreach (var tabVm in cacheHistory.Select(s => GetTab(s.File, false, s.Start)))
            {
                history.Add(tabVm);
            }

            Task.Run(() =>
            {
                if (cacheHistory?.Any() == true)
                {
                    // Загрузить из базы последнюю историю
                    var dbItems = new DbHistory().LoadHistoryFiles(cacheHistory.Last().Start);
                    dbItems.ToObservable()
                        .ObserveOn(dispatcher)
                        .Select(s => GetTab(s.DocPath, false, s.Start))
                        .Subscribe(t => { history.Add(t); });

                    // Дозаписать историю
                    cacheHistory.AddRange(dbItems.Select(GetHistoryTab));
                }
                else
                {
                    var dbItems = new DbHistory().LoadHistoryFiles();
                    dbItems.ToObservable()
                        .ObserveOn(dispatcher)
                        .Select(s => GetTab(s.DocPath, false, s.Start))
                        .Subscribe(t => { history.Add(t); });
                    cacheHistory = dbItems.Select(GetHistoryTab).ToList();
                }
                HistoryModel.SaveHistoryCache(cacheHistory);
            });
        }

        private HistoryTab GetHistoryTab(StatEvents item)
        {
            return new HistoryTab { File = item.DocPath, Start = item.Start };
        }
    }
}
