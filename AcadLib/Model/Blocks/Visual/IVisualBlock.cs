using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AcadLib.Blocks.Visual
{
    public interface IVisualBlock
    {
        string Name { get; set; }
        ImageSource Image { get; set; }
    }
}
