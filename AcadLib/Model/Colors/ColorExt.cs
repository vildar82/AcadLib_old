using Autodesk.AutoCAD.Colors;
using NetLib;

namespace AcadLib.Colors
{
    public static class ColorExt
    {
        public static string AcadColorToStrig(this Color color)
        {
            return color.ColorValue.ColorToString();
        }
        public static Color AcadColorFeomString(this string color)
        {
            return Color.FromColor(color.StringToColor());
        }
    }
}
