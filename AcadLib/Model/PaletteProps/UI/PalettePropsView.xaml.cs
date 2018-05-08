namespace AcadLib.PaletteProps.UI
{
    /// <summary>
    /// Interaction logic for PalettePropsView.xaml
    /// </summary>
    public partial class PalettePropsView
    {
        public PalettePropsView(PalettePropsVM propsVm)
        {
            InitializeComponent();
            DataContext = propsVm;
        }
    }
}
