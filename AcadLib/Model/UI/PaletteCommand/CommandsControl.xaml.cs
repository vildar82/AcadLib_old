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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AcadLib.PaletteCommands
{
    /// <summary>
    /// Логика взаимодействия для CommandsControl.xaml
    /// </summary>
    public partial class CommandsControl : UserControl
    {
        public CommandsControl()
        {
            InitializeComponent();
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;                        
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception.HResult == -2146233079) return;
            Logger.Log.Error("CommandsControl.Dispatcher_UnhandledException: " + e.Exception.ToString());
            e.Handled = true;
        }

        //private void ListBoxCommands_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    var selComm = ListBoxCommands.SelectedItem as PaletteCommand;
        //    if (selComm == null) return;
        //    selComm.Execute();            
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selComm = ((FrameworkElement)sender).DataContext as PaletteCommand;
            if (selComm == null) return;
            selComm.Execute();
        }
    }
}
