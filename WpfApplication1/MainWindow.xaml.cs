using NetLib.WPF;

namespace WpfApplication1
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow() : base(GetModel())
        {
            InitializeComponent();
        }

        private static BaseViewModel GetModel()
        {
            return new Model();
        }

        //private void RowClick(object sender, KeyEventArgs e)
        //{
        //    if (sender is DataGridRow r)
        //    {
        //        var tab = (Item)r.DataContext;
        //        tab.Restore = !tab.Restore;
        //    }
        //}

        private void DgClick(object sender, MouseButtonEventArgs e)
        {
            //if (sender is DataGrid dg)
            //{
            //    var sel = dg.SelectedItem;
            //    if (sel is Item i)
            //    {
            //        i.Restore = !i.Restore;
            //        e.Handled = true;
            //    }
            //}
        }

        private void RowClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow r)
            {
                var tab = (Item)r.DataContext;
                tab.Restore = !tab.Restore;
                e.Handled = true;
            }
        }

        private void RowSelected(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow r)
            {
                var tab = (Item)r.DataContext;
                tab.Restore = !tab.Restore;
                e.Handled = true;
            }
        }

        private void CellClick(object sender, MouseButtonEventArgs e)
        {
            //if (sender is DataGridCell cell && cell.DataContext is Item i)
            //{
            //    i.Restore = !i.Restore;
            //    e.Handled = true;
            //}
        }
    }
}
