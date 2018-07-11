namespace AcadLib.Utils.Tabs.UI
{
    using System;
    using System.Windows.Controls;
    using NetLib.WPF.Data;

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

        private void EventSetter_OnHandler(object sender, ToolTipEventArgs e)
        {
            if ((sender as DataGridRow)?.DataContext is TabVM tabVM && tabVM.Err == null && tabVM.Image == null)
            {
                try
                {
                    tabVM.Image = NetLib.IO.Path.GetThumbnail(tabVM.File).ConvertToBitmapImage();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }
    }
}
