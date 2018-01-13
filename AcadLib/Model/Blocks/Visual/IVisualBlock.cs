using System.Windows.Input;
using System.Windows.Media;

namespace AcadLib.Blocks.Visual
{
    public interface IVisualBlock
    {
        string Group { get; set; }
        string Name { get; set; }
        ImageSource Image { get; set; }
        string File { get; set; }
        ICommand Redefine { get; set; }
    }
}
