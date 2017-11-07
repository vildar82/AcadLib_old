using System.Windows;
using AcadLib.WPF.Controls;

namespace TestAcadlib.WPF
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1 (Class1 model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
