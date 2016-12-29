using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.Errors
{
    public class ErrorsViewModel : ViewModelBase
    {
        public ErrorsViewModel()
        {

        }

        public ErrorsViewModel(List<Error> errors)
        {
            // Группировка ошибок
            Errors = new ObservableCollection<ErrorModel>(errors.GroupBy(g=>g).Select(s=> new ErrorModel(s.ToList())).ToList());
        }

        public ObservableCollection<ErrorModel> Errors { get; set; }
        public bool IsDialog { get { return isDialog; } set { isDialog = value; RaisePropertyChanged(); } }
        bool isDialog;
    }
}
