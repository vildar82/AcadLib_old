using JetBrains.Annotations;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AcadLib.WPF.Converters
{
    [Obsolete]
    [PublicAPI]
    [ValueConversion(typeof(int), typeof(double))]
    [ValueConversion(typeof(double), typeof(double))]
    public class MinToHoursConverter : ConvertorBase
    {
        [NotNull]
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var min = System.Convert.ToInt32(value);
            var hours = NetLib.DoubleExt.Round(min / 60d,1);
            return hours;
        }
    }
}