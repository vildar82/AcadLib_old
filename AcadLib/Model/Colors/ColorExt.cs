using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
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

        public static Color GetEntityColorAbs(this Entity ent)
        {
            var color = ent.Color;
            if (color.IsByLayer)
            {
                var layer = ent.LayerId.GetObject<LayerTableRecord>();
                color = layer.Color;
            }
            return color;
        }
    }
}
