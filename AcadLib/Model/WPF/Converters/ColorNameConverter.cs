using JetBrains.Annotations;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AcadLib.WPF.Converters
{
    [ValueConversion(typeof(Autodesk.AutoCAD.Colors.Color), typeof(string))]
    public class ColorNameConverter : ConvertorBase
    {
        [NotNull]
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Autodesk.AutoCAD.Colors.Color ac)
            {
                return ac.ToString();
            }
            return "Нет";
        }
    }
}
