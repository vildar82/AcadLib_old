namespace AcadLib.PaletteProps
{
    using System.Windows.Data;
    using System.Windows.Input;
    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for IntValueView.xaml
    /// </summary>
    public partial class IntView
    {
        public IntView(IntVM vm)
            : base(vm, false)
        {
            InitializeComponent();
        }
    }
}