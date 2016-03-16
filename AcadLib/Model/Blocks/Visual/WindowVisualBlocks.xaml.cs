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

namespace AcadLib.Blocks.Visual
{
    /// <summary>
    /// Логика взаимодействия для WindowVisualBlocks.xaml
    /// </summary>
    public partial class WindowVisualBlocks : Window
    {
        public VisualBlock Selected;

        public WindowVisualBlocks(List<VisualBlock> visuals)
        {
            InitializeComponent();

            VisualModel model = new VisualModel();
            DataContext = model;
            model.LoadData(visuals);

            ListBoxVisuals.MouseLeftButtonUp += ListBoxVisuals_MouseLeftButtonUp;
            //lvwBlocks.MouseLeftButtonUp += ListBoxVisuals_MouseLeftButtonUp;            
        }

        private void ListBoxVisuals_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Selected = ListBoxVisuals.SelectedItem as VisualBlock;
            //Selected = lvwBlocks.SelectedItem as VisualBlock;
            DialogResult = true;
            this.Hide();
        }
    }
}
