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
        public ErrorModel parentErr;
        public event EventHandler<bool> SelectionChanged;

        public ErrorModel(IError err, ErrorModel parentErr) : this(err.Yield().ToList())
        {
            this.parentErr = parentErr;
        }

        public ErrorModel(List<IError> sameErrors)
        {
            Count = sameErrors.Count();
            firstErr = sameErrors.First();
            Message = firstErr.Message;
            if (firstErr.Icon != null)
            {
                Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    firstErr.Icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            Show = new RelayCommand(OnShowExecute);
            if (sameErrors.Skip(1).Any())
            {
                SameErrors = new ObservableCollection<ErrorModel>();
                for (int i =0; i<sameErrors.Count; i++)
                {
                    SameErrors.Add(new ErrorModel(sameErrors[i], this));
                }                
            }
            HasShow = firstErr.CanShow;                        
        }        

        public IError Error { get { return firstErr; } }
        public ObservableCollection<ErrorModel> SameErrors { get; set; }        
        public string Message { get; set; }
        public BitmapSource Image { get; set; }
        public RelayCommand Show { get; set; }
        public bool IsExpanded {
            get { return isExpanded; }
            set {
                if (SameErrors != null)
                {
                    isExpanded = value;
                    RaisePropertyChanged();
                }
            }
        }
        bool isExpanded;

        public bool HasShow { get; set; }
        public bool ShowCount { get { return Count != 1; } }
        public bool IsSelected { get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged(); SelectionChanged?.Invoke(this, value); } }
        bool isSelected;

        public int Count { get; set; }

        private void OnShowExecute()
        {            
            firstErr.Show();
            IsExpanded = !IsExpanded;
            IsSelected = true;
        }
    }
}
