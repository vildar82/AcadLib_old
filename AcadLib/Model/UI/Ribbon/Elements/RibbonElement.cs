namespace AcadLib.UI.Ribbon.Elements
{
    using System.Windows.Input;
    using System.Windows.Media;

    public class RibbonElement : IRibbonElement
    {
        public string Tab { get; set; }

        public string Panel { get; set; }

        public ICommand Command { get; set; }

        public string Name { get; set; }

        public ImageSource LargeImage { get; set; }

        public ImageSource Image { get; set; }

        public string Description { get; set; }
    }
}