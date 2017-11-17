using AcadLib.WPF;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow() : base(GetModel())
        {
            InitializeComponent();
        }

        private static BaseViewModel GetModel()
        {
            return new Model();
        }
    }
}
