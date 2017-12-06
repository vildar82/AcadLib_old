using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.AutoCAD.Windows;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using MicroMvvm;

namespace AcadLib.WPF.Controls
{
    /// <summary>
    /// Кнопка выбора цвета.
    /// <controls:AcadColorPick Color="{Binding Color}"/> 
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class AcadColorPick : INotifyPropertyChanged
    {
        public AcadColorPick()
        {
            InitializeComponent();
            CanClearColor = true;
        }

        [CanBeNull]
        public Autodesk.AutoCAD.Colors.Color Color
        {
            get => (Autodesk.AutoCAD.Colors.Color)GetValue(ColorProperty);
            set
            {
                SetValue(ColorProperty, value);
                CanClearColor = Color != null;
            }
        }

        public bool CanClearColor
        {
            get => canClearColor;
            set
            {
                if (value == canClearColor) return;
                canClearColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Цвет (AutoCAD)
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Autodesk.AutoCAD.Colors.Color), typeof(AcadColorPick),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private bool canClearColor;

        private void SelectColor(object sender, RoutedEventArgs e)
        {
            var colorDlg = new ColorDialog { Color = Color };
            if (colorDlg.ShowModal() == true)
            {
                Color = colorDlg.Color;
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            Color = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
