using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using NetLib;

namespace AcadLib.Colors
{
    public static class ColorExt
    {
        public static string AcadColorToStrig(this Color color)
        {
            return color?.ColorValue.ColorToString() ?? "";
        }
        public static Color AcadColorFeomString(this string color)
        {
            return Color.FromColor(color.StringToColor());
        }

        /// <summary>
        /// Определение цвета объекта - если ПоСлою, то возвращает цвет слоя.
        /// </summary>
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

        /// <summary>
        /// Цвет в строку - индекс цвета, или r,g,b.
        /// </summary>
        public static string AcadColorToString2(this Color color)
        {
            if (color == null) return "";
            if (color.IsByLayer) return "256";
            if (color.IsByBlock) return "0";
            if (color.IsByAci) return color.ColorIndex.ToString();
            return $"{color.Red},{color.Green},{color.Blue}";
        }

        /// <summary>
        /// строку в цвет - из color?.ToString();
        /// </summary>
        public static Color AcadColorFromString2(this string color)
        {
            if (color.IsNullOrEmpty()) return null;
            // Index
            if (short.TryParse(color, out var colorIndex))
            {
                return Color.FromColorIndex(ColorMethod.ByAci, colorIndex);
            }
            // RGB
            var rgb = color.Split(',');
            if (rgb.Length == 3)
            {
                return Color.FromRgb(byte.Parse(rgb[0]), byte.Parse(rgb[1]), byte.Parse(rgb[2]));
            }
            return null;
        }
    }
}
