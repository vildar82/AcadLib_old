using JetBrains.Annotations;
using System.Windows;
using System.Windows.Controls;

namespace AcadLib.PaletteCommands.UI
{
    /// <summary>
    /// Логика взаимодействия для CommandsControl.xaml
    /// </summary>
    public partial class CommandsControl
    {
        public CommandsControl()
        {
            InitializeComponent();
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, [NotNull] System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is UserBreakException)
            {
                throw e.Exception;
            }

            if (e.Exception.HResult != -2146233079)
            {
                Logger.Log.Error("CommandsControl.Dispatcher_UnhandledException: " + e.Exception.ToString());
            }
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
            if (!(((FrameworkElement)sender).DataContext is PaletteCommand selComm)) return;
            selComm.Execute();
        }
    }
}
