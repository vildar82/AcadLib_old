using AcadLib.Errors.UI;
using AcadLib.Layers;
using Autodesk.AutoCAD.DatabaseServices;
using MicroMvvm;
using NetLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AcadLib.Errors
{
    public class ErrorModel : ModelBase
    {
        private bool isExpanded;
        private bool isSelected;
        private readonly IError firstErr;
        public readonly ErrorModel parentErr;
        private readonly ErrorsViewModel errorsModel;
        public event EventHandler<bool> SelectionChanged;

        public ErrorModel(IError err, ErrorModel parentErr, ErrorsViewModel errorsModel)
            : this(err.Yield().ToList(), errorsModel, false)
        {
            this.parentErr = parentErr;
        }

        public ErrorModel(List<IError> sameErrors, ErrorsViewModel errorsModel, bool isGroup)
        {
            this.errorsModel = errorsModel;
            Count = sameErrors.Count;
            firstErr = sameErrors.First();
            Background = firstErr.Background.IsEmpty ? Color.LightGray : firstErr.Background;
            if (sameErrors.Count == 1)
            {
                Message = !isGroup && firstErr.Message.Length > firstErr.Group.Length
                    ? firstErr.Message.Substring(firstErr.Group.Length)
                    : firstErr.Message;
                AddButtons = firstErr.AddButtons;
            }
            else
            {
                Message = firstErr.Group;
            }
            if (firstErr.Icon != null)
            {
                Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    firstErr.Icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            Show = new RelayCommand(OnShowExecute);
            if (sameErrors.Skip(1).Any())
            {
                SameErrors = new ObservableCollection<ErrorModel>(sameErrors.Select(s => new ErrorModel(s, this, errorsModel)));
            }
            HasShow = firstErr.CanShow;
            DeleteError = new RelayCommand(DeleteErrorExec, () => Error?.HasEntity == true);
            // Добавить кнопку, для отрисовки визуализации на чертежа
            if (HasVisuals)
            {
                if (AddButtons == null) AddButtons = new List<ErrorAddButton>();
                var visCommand = new RelayCommand(AddVisualsToDrawing, () => Error?.Visuals?.Any() == true);
                var visButton = new ErrorAddButton
                {
                    Name = "Отрисовка",
                    Tooltip = "Добавить визуализацию ошибки в чертеж.",
                    Click = visCommand
                };
                AddButtons.Add(visButton);
            }
        }

        public IError Error => firstErr;
        public ObservableCollection<ErrorModel> SameErrors { get; set; }
        public string Message { get; set; }
        public List<ErrorAddButton> AddButtons { get; set; }
        public BitmapSource Image { get; set; }
        public RelayCommand Show { get; set; }
        public RelayCommand DeleteError { get; set; }
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
        public bool HasShow { get; set; }
        public bool ShowCount => Count != 1;
        public bool IsSelected
        {
            get => isSelected;
            set { isSelected = value; RaisePropertyChanged(); SelectionChanged?.Invoke(this, value); }
        }
        public int Count { get; set; }
        public bool HasAddButtons => AddButtons?.Any() == true;
        public bool HasVisuals => Error?.Visuals?.Any() == true;
        public Color Background { get; set; }

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
            using (var t = db.TransactionManager.StartTransaction())
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

        private void AddVisualsToDrawing()
        {
            try
            {
                var doc = AcadHelper.Doc;
                var db = doc.Database;
                using (doc.LockDocument())
                using (var t = db.TransactionManager.StartTransaction())
                {
                    var layerVisual = LayerExt.CheckLayerState("visuals");
                    var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject<BlockTableRecord>(OpenMode.ForWrite);
                    var fEnt = Error.Visuals.First();
                    Extents3d fEntExt = new Extents3d();
                    ObjectId fEntId = ObjectId.Null;
                    foreach (var entity in Error.Visuals)
                    {
                        var entClone = (Entity)entity.Clone();
                        entClone.LayerId = layerVisual;
                        ms.AppendEntity(entClone);
                        t.AddNewlyCreatedDBObject(entClone, true);
                        if (fEnt == entity)
                        {
                            fEntId = entClone.Id;
                            try
                            {
                                fEntExt = entClone.GeometricExtents;
                            }
                            catch
                            {
                                //
                            }
                        }
                        entity.Dispose();
                    }
                    if (!Error.HasEntity && !fEntId.IsNull)
                    {
                        Error.HasEntity = true;
                        Error.IdEnt = fEntId;
                        Error.Extents = fEntExt;
                        HasShow = true;
                    }
                    Error.Visuals = new List<Entity>();
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
