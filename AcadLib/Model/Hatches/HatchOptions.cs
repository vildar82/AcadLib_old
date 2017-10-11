using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Hatches
{
    public class HatchOptions
    {
        public HatchOptions()
        {
            
        }

        public HatchOptions(Hatch h)
        {
            PatternName = h.PatternName;
            PatternType = h.PatternType;
            PatternScale = h.PatternScale;
            PatternAngle = h.PatternAngle;
        }

        public string PatternName { get; set; }
        public HatchPatternType PatternType { get; set; }
        public double? PatternScale { get; set; }
        public double? PatternAngle { get; set; }
    }
}
