using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AcadLib.WPF.Controls
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Interaction logic for TransparenceSlider.xaml
    /// </summary>
    public partial class TransparenceSlider : UserControl
    {
        public TransparenceSlider()
        {
            InitializeComponent();
        }

        public byte Transparence
        {
            get => (byte)GetValue(TransparenceProperty);
            set => SetValue(TransparenceProperty, value);
        }

        public static readonly DependencyProperty TransparenceProperty =
            DependencyProperty.Register("Transparence", typeof(byte), typeof(TransparenceSlider),
                new FrameworkPropertyMetadata(default(byte), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    }
}
