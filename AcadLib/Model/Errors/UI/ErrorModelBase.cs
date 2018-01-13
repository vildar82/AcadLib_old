using AcadLib.Errors.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using Visibility = System.Windows.Visibility;

// ReSharper disable once CheckNamespace
namespace AcadLib.Errors
{
    [PublicAPI]
    public abstract class ErrorModelBase : ReactiveObject
    {
        protected IError firstErr;
        private bool isSelected;

        public event EventHandler<bool> SelectionChanged;

        public List<ErrorAddButton> AddButtons { get; set; }

        public Color Background { get; set; }

        public IError Error => firstErr;

        public bool HasAddButtons => AddButtons?.Any() == true;

        public bool HasShow { get; set; }

        public bool HasVisuals => Error?.Visuals?.Any() == true;

        public BitmapSource Image { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set {
                isSelected = value;
                this.RaisePropertyChanged();
                SelectionChanged?.Invoke(this, value);
            }
        }

        public Thickness MarginHeader { get; set; }

        public string Message { get; set; }

        public ReactiveCommand Show { get; set; }

        public bool ShowCount { get; set; }

        public Visibility VisibilityCount { get; set; }

        public ErrorModelBase(IError err)
        {
            MarginHeader = new Thickness(2);
            firstErr = err;
            Show = ReactiveCommand.Create(OnShowExecute);
            if (firstErr.Icon != null)
            {
                Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    firstErr.Icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            HasShow = firstErr.CanShow;
            Background = firstErr.Background;
        }

        protected virtual void OnShowExecute()
        {
            firstErr.Show();
        }
    }
}