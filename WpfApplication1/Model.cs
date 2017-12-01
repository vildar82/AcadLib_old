using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AcadLib.Errors;
using AcadLib.WPF;
using NetLib.WPF;
using PIK_GP_Civil.Parkings.Area;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WpfApplication1.Dialog;

namespace WpfApplication1
{
    public class Model : BaseViewModel
    {
        public Model()
        {
            ShowDialog = CreateCommand(ShowDialogExecute);
            TestErrors = CreateCommand(TestErrorsExec);
        }

        [Reactive] public byte Transparence { get; set; } = 50;
        public ReactiveCommand ShowDialog { get; set; }
        public ICommand TestErrors { get; set; }

        private void ShowDialogExecute()
        {
            var dialogVM = new DialogViewModel();
            var dialogView = new DialogView(dialogVM);
            if (dialogView.ShowDialog() == true)
            {
                MessageBox.Show(dialogVM.Value);
            }
        }

        private void TestErrorsExec()
        {
            var errors = new List<IError>();
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    errors.Add(new Error($"Сообщение об ошибке {i}"));
                }
            }
            var errVM = new ErrorsViewModel(errors) { IsDialog = true };
            var errView = new ErrorsView(errVM);
            errView.Show();
        }
    }
}
