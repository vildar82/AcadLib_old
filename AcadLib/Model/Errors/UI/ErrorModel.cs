using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using NetLib;
using AcadLib;

namespace AcadLib.Errors
{
    public class ErrorModel : ModelBase
    {
        private IError firstErr;

        public ErrorModel(IError err) : this(err.Yield())
        {            
        }

        public ErrorModel(IEnumerable<IError> sameErrors)
        {
            firstErr = sameErrors.First();
            Message = firstErr.Message;
            if (firstErr.Icon != null)
            {
                Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    firstErr.Icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            Show = new RelayCommand(OnShowExecute);
            if (sameErrors.Skip(1).Any())
                SameErrors = new ObservableCollection<ErrorModel>(sameErrors.Skip(1).Select(s=>new ErrorModel(s)));
        }        

        public ObservableCollection<ErrorModel> SameErrors { get; set; }        
        public string Message { get; set; }
        public BitmapSource Image { get; set; }
        public RelayCommand Show { get; set; }
        public bool IsExpanded { get { return isExpanded; } set { isExpanded = value; RaisePropertyChanged(); } }
        bool isExpanded;

        private void OnShowExecute()
        {            
            firstErr.Show();
            IsExpanded = !IsExpanded;
        }
    }
}
