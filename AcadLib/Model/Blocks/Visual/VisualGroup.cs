using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Blocks.Visual
{
    public class VisualGroup
    {
        public string Name { get; set; }
        public List<IVisualBlock> Blocks { get; set; }
    }
}
