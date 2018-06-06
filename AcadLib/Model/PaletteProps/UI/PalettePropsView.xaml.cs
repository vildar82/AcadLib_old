using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AcadLib.PaletteProps.UI
{
    /// <summary>
    /// Interaction logic for PalettePropsView.xaml
    /// </summary>
    public partial class PalettePropsView
    {
        public PalettePropsView(PalettePropsVM propsVm) : base(propsVm)
        {
            InitializeComponent();
        }

        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = MouseWheelEvent,
                    Source = sender
                };
                var parent = ((Control)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }
    }
}
