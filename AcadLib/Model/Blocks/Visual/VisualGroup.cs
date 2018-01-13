using System.Collections.Generic;

namespace AcadLib.Blocks.Visual
{
    public class VisualGroup
    {
        public string Name { get; set; }
        public List<IVisualBlock> Blocks { get; set; }
    }
}
