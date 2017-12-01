using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AcadLib.Errors;
using AcadLib.Errors.UI;
using AcadLib.WPF;
using NetLib;
using NetLib.WPF;
using PIK_GP_Civil.Parkings.Area;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WpfApplication1.Dialog;

namespace WpfApplication1
{
    public class Model : BaseViewModel
    {
        private Random r = new Random();
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
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    var err = new Error(Enumerable.Range(0, i).JoinToString());
                    //if (i.IsOdd())
                    {
                        err.Background = Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
                        err.AddButtons = new List<ErrorAddButton>
                        {
                            new ErrorAddButton {Name = "Test", Tooltip = "Tttt"},
                            new ErrorAddButton {Name = "Test2"},
                        };
                    }
                    errors.Add(err);
                }
            }
            var errVM = new ErrorsViewModel(errors) { IsDialog = true };
            var errView = new ErrorsView(errVM);
            errView.Show();
        }
    }
}
