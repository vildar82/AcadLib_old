// ReSharper disable once CheckNamespace
namespace AcadLib.Errors
{
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Autodesk.AutoCAD.ApplicationServices;
    using JetBrains.Annotations;
    using NetLib;
    using Visual;

    /// <summary>
    /// Логика взаимодействия для WindowErrors.xaml
    /// </summary>
    public partial class ErrorsView
    {
        private readonly Document doc;
        private readonly VisualTransientSimple errorsVisual;

        public ErrorsView([NotNull] ErrorsViewModel errVM) : base(errVM)
        {
            doc = AcadHelper.Doc;
            InitializeComponent();
            DataContext = errVM;
            KeyDown += ErrorsView_KeyDown;
            Closed += ErrorsView_Closed;
            var visualsEnts = errVM.ErrorsOrig.SelectManyNulless(s => s.Visuals).ToList();
            if (visualsEnts.Any())
            {
                errorsVisual = new VisualTransientSimple(visualsEnts) { VisualIsOn = true };
            }
        }

        ~ErrorsView()
        {
            Dispose();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Dispose();
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            var subject = $"Обращение по работе команды {CommandStart.CurrentCommand}";
            Process.Start($"mailto:khisyametdinovvt@pik.ru?subject={subject}");
        }

        private void Dispose()
        {
            if (AcadHelper.Doc != doc)
                return;
            using (doc.LockDocument())
            {
                errorsVisual?.Dispose();
            }
        }

        private void ErrorsView_Closed(object sender, System.EventArgs e)
        {
            Dispose();
        }

        private void ErrorsView_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;

                case Key.Delete:
                    var model = (ErrorsViewModel)DataContext;
                    model.DeleteSelectedErrors();
                    break;
            }
        }

        private void HeaderTemplateStretchHack(object sender, RoutedEventArgs e)
        {
            ((ContentPresenter)((Grid)sender).TemplatedParent).HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}