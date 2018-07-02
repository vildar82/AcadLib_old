namespace AcadLib.Utils.Tabs.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for TabsView.xaml
    /// </summary>
    public partial class TabsView
    {
        public TabsView(TabsVM vm)
            : base(vm)
        {
            InitializeComponent();
        }

        private void Row_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridRow r)
            {
                var tab = (TabVM)r.DataContext;
                tab.Restore = !tab.Restore;
            }
        }

        private void Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridRow r)
            {
                var tab = (TabVM)r.DataContext;
                ((TabsVM)DataContext).OpenFileExec(tab);
            }
        }
    }
}
