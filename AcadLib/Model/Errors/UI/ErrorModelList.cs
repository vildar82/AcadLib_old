using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AcadLib.Errors.UI
{
    public class ErrorModelList : ErrorModelBase
    {
        public ErrorModelList(List<IError> sameErrors) : base(sameErrors.First())
        {
            VisibilityCount = Visibility.Visible;
            firstErr = sameErrors.First();
            Message = firstErr.Group;
            Header = new ErrorModelOne(firstErr, null)
            {
                AddButtons = null,
                MarginHeader = new Thickness(2),
                Parent = this,
                ShowCount = true
            };
            SameErrors = new ObservableCollection<ErrorModelBase>(
                sameErrors.Select(s => new ErrorModelOne(s, this)));
        }
        [Reactive] public ObservableCollection<ErrorModelBase> SameErrors { get; set; }
        public ErrorModelOne Header { get; set; }
        [Reactive] public bool IsExpanded { get; set; }

        protected override void OnShowExecute()
        {
            base.OnShowExecute();
            IsExpanded = !IsExpanded;
        }
    }
}
