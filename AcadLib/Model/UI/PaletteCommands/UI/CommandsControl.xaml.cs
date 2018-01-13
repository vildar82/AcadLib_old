using JetBrains.Annotations;
using System.Windows;

// ReSharper disable once CheckNamespace
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

        private static void Dispatcher_UnhandledException(object sender, [NotNull] System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#pragma warning disable 618
            if (e.Exception is UserBreakException)
#pragma warning restore 618
            {
                throw e.Exception;
            }

            if (e.Exception.HResult != -2146233079)
            {
                Logger.Log.Error("CommandsControl.Dispatcher_UnhandledException: " + e.Exception);
            }
            e.Handled = true;
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(((FrameworkElement)sender).DataContext is PaletteCommand selComm)) return;
            selComm.Execute();
        }
    }
}