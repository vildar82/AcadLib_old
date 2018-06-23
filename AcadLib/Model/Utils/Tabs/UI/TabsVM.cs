namespace AcadLib.Utils.Tabs.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using Data;
    using NetLib.WPF;
    using ReactiveUI;

    public class TabsVM : BaseViewModel
    {
        private Tabs _tabs;

        public TabsVM(Tabs tabs)
        {
            _tabs = tabs;
            Tabs = tabs.Drawings.Select(GetTab).ToList();
            Ok = CreateCommand(OkExec);
        }

        public List<TabVM> Tabs { get; set; }

        public ReactiveCommand Ok { get; set; }

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
            _tabs.Drawings = Tabs.Where(w => w.Restore).Select(s => s.File).ToList();
            DialogResult = true;
        }
    }
}
