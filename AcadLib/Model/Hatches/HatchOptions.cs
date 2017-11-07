using Autodesk.AutoCAD.Colors;
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
            BackgroundColor = h.BackgroundColor;
        }

        public string PatternName { get; set; }
        public HatchPatternType PatternType { get; set; }
        public double? PatternScale { get; set; }
        public double? PatternAngle { get; set; }
        public Color BackgroundColor { get; set; }
    }
}
