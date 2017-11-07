using Autodesk.AutoCAD.Windows;
using System.Windows;
using System.Windows.Controls;

namespace AcadLib.WPF.Controls
{
    /// <summary>
    /// Кнопка выбора цвета.
    /// <controls:AcadColorPick Color="{Binding Color}"/> 
    /// </summary>
    public partial class AcadColorPick : UserControl
    {
        public AcadColorPick()
        {
            InitializeComponent();
        }

        public Autodesk.AutoCAD.Colors.Color Color
        {
            get => (Autodesk.AutoCAD.Colors.Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        /// <summary>
        /// Цвет (AutoCAD)
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Autodesk.AutoCAD.Colors.Color), typeof(AcadColorPick),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
