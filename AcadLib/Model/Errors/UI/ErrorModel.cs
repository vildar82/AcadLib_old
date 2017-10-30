using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Autodesk.AutoCAD.DatabaseServices;
using NetLib;

namespace AcadLib.Errors
{
    public class ErrorModel : ModelBase
    {
        private readonly IError firstErr;
        public readonly ErrorModel parentErr;
        private readonly ErrorsViewModel errorsModel;
        public event EventHandler<bool> SelectionChanged;

        public ErrorModel(IError err, ErrorModel parentErr, ErrorsViewModel errorsModel) : this(err.Yield().ToList(), errorsModel)
        {
            this.parentErr = parentErr;
        }

        public ErrorModel(List<IError> sameErrors,ErrorsViewModel errorsModel)
        {
            this.errorsModel = errorsModel;
            Count = sameErrors.Count;
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
                for (var i =0; i<sameErrors.Count; i++)
                {
                    SameErrors.Add(new ErrorModel(sameErrors[i], this, errorsModel));
                }                
            }
            HasShow = firstErr.CanShow;
            DeleteError = new RelayCommand(DeleteErrorExec, () => Error?.HasEntity == true);
        }

        public IError Error { get { return firstErr; } }
        public ObservableCollection<ErrorModel> SameErrors { get; set; }        
        public string Message { get; set; }
        public BitmapSource Image { get; set; }
        public RelayCommand Show { get; set; }
        public RelayCommand DeleteError { get; set; }
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
            //IsSelected = true;
        }

        private void DeleteErrorExec()
        {
            if (parentErr != null)
            {
                parentErr.SameErrors.Remove(this);
            }
            else
            {
                errorsModel.Errors.Remove(this);
            }
            if (Error == null)
            {
                throw new ArgumentException("Ошибка не найдена.");
            }
            if (!Error.IdEnt.IsValidEx())
            {
                throw new Exception($"Элемент ошибки не валидный. Возможно был удален.");
            }
            var doc = AcadHelper.Doc;
            var db = doc.Database;
            if (Error.IdEnt.Database != db)
            {
                throw new Exception($"Переключитесь на чертеж '{Path.GetFileName(doc.Name)}'");
            }
            using (doc.LockDocument())
            using(var t = db.TransactionManager.StartTransaction())
            {
                var ent = Error.IdEnt.GetObject<Entity>(OpenMode.ForWrite);
                ent?.Erase();
                if (SameErrors != null)
                {
                    foreach (var error in SameErrors)
                    {
                        ent = error.Error.IdEnt.GetObject<Entity>(OpenMode.ForWrite);
                        ent?.Erase();
                    }
                }
                t.Commit();
            }
        }
    }
}
