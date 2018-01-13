using JetBrains.Annotations;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AcadLib.WPF.Converters
{
    [Obsolete]
    [ValueConversion(typeof(int), typeof(string))]
    [ValueConversion(typeof(double), typeof(string))]
    [PublicAPI]
    public class RadiansToDegreeConverterExtension : ConvertorBase
    {
        [NotNull]
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = System.Convert.ToDouble(value);
            return NetLib.MathExt.ToDegrees(date).ToString("N2");
        }

        [NotNull]
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = System.Convert.ToDouble(value);
            return NetLib.MathExt.ToRadians(date);
        }
    }
}