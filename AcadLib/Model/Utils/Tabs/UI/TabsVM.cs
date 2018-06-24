namespace AcadLib.Utils.Tabs.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using JetBrains.Annotations;
    using NetLib.WPF;
    using ReactiveUI;

    public class TabsVM : BaseViewModel
    {
        public TabsVM([NotNull] IEnumerable<string> drawings)
        {
            Tabs = drawings.Select(GetTab).ToList();
            Ok = CreateCommand(OkExec);
        }

        public List<TabVM> Tabs { get; set; }

        public ReactiveCommand Ok { get; set; }

        public bool IsOn { get; set; } = true;

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
            var tabVM = new TabVM
            {
                Name = Path.GetFileNameWithoutExtension(tab),
                File = tab,
                Restore = true
            };
            return tabVM;
        }

        private void OkExec()
        {
            DialogResult = true;
        }
    }
}
