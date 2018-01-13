using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.AutoCAD.Windows;
using System.Windows;
using JetBrains.Annotations;

namespace AcadLib.WPF.Controls
{
    /// <summary>
    /// Кнопка выбора цвета.
    /// <controls:AcadColorPick Color="{Binding Color}"/>
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class AcadColorPick : INotifyPropertyChanged
    {
        /// <summary>
        /// Цвет (AutoCAD)
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Autodesk.AutoCAD.Colors.Color), typeof(AcadColorPick),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private bool canClearColor;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanClearColor
        {
            get => canClearColor;
            set {
                if (value == canClearColor) return;
                canClearColor = value;
                OnPropertyChanged();
            }
        }

        [CanBeNull]
        public Autodesk.AutoCAD.Colors.Color Color
        {
            get => (Autodesk.AutoCAD.Colors.Color)GetValue(ColorProperty);
            set {
                SetValue(ColorProperty, value);
                CanClearColor = Color != null;
            }
        }

        public AcadColorPick()
        {
            InitializeComponent();
            CanClearColor = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CanBeNull] [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            Color = null;
        }

        private void SelectColor(object sender, RoutedEventArgs e)
        {
            var colorDlg = new ColorDialog { Color = Color };
            if (colorDlg.ShowModal() == true)
            {
                Color = colorDlg.Color;
            }
        }
    }
}