using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AcadLib.WPF;
using PIK_GP_Civil.Parkings.Area;
using ReactiveUI;
using WpfApplication1.Dialog;

namespace WpfApplication1
{
    public class Model : BaseViewModel
    {
        public Model()
        {
            ShowDialog = CreateCommand(ShowDialogExecute);
        }

        public ReactiveCommand ShowDialog { get; set; }

        private void ShowDialogExecute()
        {
            var dialogVM = new DialogViewModel();
            var dialogView = new DialogView(dialogVM);
            if (dialogView.ShowDialog() == true)
            {
                MessageBox.Show(dialogVM.Value);
            }
        }
    }
}
