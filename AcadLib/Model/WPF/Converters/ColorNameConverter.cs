using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AcadLib.WPF.Converters
{
    [ValueConversion(typeof(Autodesk.AutoCAD.Colors.Color), typeof(string))]
    public class ColorNameConverter : ConvertorBase
    {
        public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Autodesk.AutoCAD.Colors.Color ac)
            {
                return ac.ToString();
            }
            return "Нет";
        }
    }
}
