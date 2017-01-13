using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AcadLib.Errors
{
    /// <summary>
    /// Логика взаимодействия для WindowErrors.xaml
    /// </summary>
    public partial class ErrorsView : Window
    {
        public ErrorsView(ErrorsViewModel errVM)
        {                        
            InitializeComponent();
            DataContext = errVM;
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer && !e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                ((Control)sender).RaiseEvent(eventArg);                
            }
        }        

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
