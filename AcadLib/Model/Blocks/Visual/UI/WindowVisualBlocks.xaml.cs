using System.Windows;
using System.Windows.Input;

namespace AcadLib.Blocks.Visual.UI
{
    /// <summary>
    /// Логика взаимодействия для WindowVisualBlocks.xaml
    /// </summary>
    public partial class WindowVisualBlocks
    {
        public VisualBlock Selected;

        public WindowVisualBlocks(VisualBlocksViewModel vm) : base(vm)
        {
            InitializeComponent();
            //lbVisuals.MouseLeftButtonUp += ListBoxVisuals_MouseLeftButtonUp;
            //lvwBlocks.MouseLeftButtonUp += ListBoxVisuals_MouseLeftButtonUp;      
            KeyDown += WindowVisualBlocks_KeyDown;
        }

        private void WindowVisualBlocks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
        }

        private void ButtonInsert_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
