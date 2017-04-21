using Autodesk.AutoCAD.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
