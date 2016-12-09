using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AcadLib.Blocks.Visual.UI
{
    /// <summary>
    /// Логика взаимодействия для WindowVisualBlocks.xaml
    /// </summary>
    public partial class WindowVisualBlocks : Window
    {
        public VisualBlock Selected;

        public WindowVisualBlocks(VisualBlocksViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;            

            //lbVisuals.MouseLeftButtonUp += ListBoxVisuals_MouseLeftButtonUp;
            //lvwBlocks.MouseLeftButtonUp += ListBoxVisuals_MouseLeftButtonUp;            
        }
       

        private void ButtonInsert_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
