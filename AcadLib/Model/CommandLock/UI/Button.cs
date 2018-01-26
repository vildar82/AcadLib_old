using System.Windows.Input;

namespace AcadLib.CommandLock.UI
{
    public class Button
    {
        public ICommand Command { get; set; }
        public bool IsCancel { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
    }
}