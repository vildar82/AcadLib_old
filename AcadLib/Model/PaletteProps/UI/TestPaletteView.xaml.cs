using AcadLib.UI.Properties;

namespace AcadLib.PaletteProps.UI
{
    /// <summary>
    /// Interaction logic for TestPaletteView.xaml
    /// </summary>
    public partial class TestPaletteView
    {
        public TestPaletteView(PalettePropsView palette) 
        {
            InitializeComponent();
            Presenter.Content = palette;
        }
    }
}
