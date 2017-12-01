using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AcadLib.Visual;
using Autodesk.AutoCAD.ApplicationServices;
using NetLib;

namespace AcadLib.Errors
{
    /// <summary>
    /// Логика взаимодействия для WindowErrors.xaml
    /// </summary>
    public partial class ErrorsView
    {
        private readonly VisualTransientSimple errorsVisual;
        private readonly Document doc;

        public ErrorsView(ErrorsViewModel errVM) : base(errVM)
        {
            doc = AcadHelper.Doc;
            InitializeComponent();
            DataContext = errVM;
            KeyDown += ErrorsView_KeyDown;
            Closed += ErrorsView_Closed;
            var visualsEnts = errVM.ErrorsOrig.SelectManyNulless(s => s.Visuals).ToList();
            if (visualsEnts.Any())
            {
                errorsVisual = new VisualTransientSimple(visualsEnts) {VisualIsOn = true};
            }
        }

        ~ErrorsView()
        {
            Dispose();
        }

        private void Dispose()
        {
            if (AcadHelper.Doc != doc) return;
            using (doc.LockDocument())
            {
                errorsVisual?.Dispose();
            }
        }

        private void ErrorsView_Closed(object sender, System.EventArgs e)
        {
            Dispose();
        }

        private void ErrorsView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key ==  Key.Delete)
            {
                var model = DataContext as ErrorsViewModel;
                model.DeleteSelectedErrors();
            }
        }
        

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Dispose();
        }        

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }        

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            var subject = $"Обращение по работе команды {CommandStart.CurrentCommand}";            
            Process.Start($"mailto:khisyametdinovvt@pik.ru?subject={subject}");
        }

        private void HeaderTemplateStretchHack(object sender, RoutedEventArgs e)
        {
            ((ContentPresenter)((Grid)sender).TemplatedParent).HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}
