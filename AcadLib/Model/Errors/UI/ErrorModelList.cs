using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MicroMvvm;

namespace AcadLib.Errors.UI
{
    public class ErrorModelList : ErrorModelBase
    {
        private bool isExpanded;

        public ErrorModelList(List<IError> sameErrors) : base(sameErrors.First())
        {
            Count = sameErrors.Count;
            firstErr = sameErrors.First();
            Message = firstErr.Group;
            Header = new ErrorModelOne(firstErr, null) {AddButtons = null};
            SameErrors = new ObservableCollection<ErrorModelBase>(
                sameErrors.Select(s => new ErrorModelOne(s, (ErrorModelList)this)));
        }

        public ErrorModelOne Header { get; set; }
        public ObservableCollection<ErrorModelBase> SameErrors { get; set; }
        public bool IsExpanded
        {
            get => isExpanded;
            set {
                if (SameErrors != null)
                {
                    isExpanded = value;
                    RaisePropertyChanged();
                }
            }
        }

        protected override void OnShowExecute()
        {
            base.OnShowExecute();
            IsExpanded = !IsExpanded;
        }
    }
}
