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
using ReactiveUI.Fody.Helpers;
using Visibility = System.Windows.Visibility;

namespace AcadLib.Errors
{
    public abstract class ErrorModelBase : ModelBase
    {
        private bool isSelected;
        protected IError firstErr;
        public event EventHandler<bool> SelectionChanged;

        public ErrorModelBase(IError err)
        {
            MarginHeader = new Thickness(2);
            firstErr = err;
            Show = new RelayCommand(OnShowExecute);
            if (firstErr.Icon != null)
            {
                Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    firstErr.Icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            HasShow = firstErr.CanShow;
            Background = firstErr.Background;
        }

        public bool ShowCount { get; set; }
        public IError Error => firstErr;
        public Thickness MarginHeader { get; set; }
        public Visibility VisibilityCount { get; set; }
        public string Message { get; set; }
        public List<ErrorAddButton> AddButtons { get; set; }
        public BitmapSource Image { get; set; }
        public RelayCommand Show { get; set; }
        
        public bool HasShow { get; set; }
        
        public bool IsSelected
        {
            get => isSelected;
            set { isSelected = value; RaisePropertyChanged(); SelectionChanged?.Invoke(this, value); }
        }
        
        public bool HasAddButtons => AddButtons?.Any() == true;
        public bool HasVisuals => Error?.Visuals?.Any() == true;
        public Color Background { get; set; }

        protected virtual void OnShowExecute()
        {
            firstErr.Show();
        }
    }
}
