namespace AcadLib.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using JetBrains.Annotations;

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
            var hours = NetLib.DoubleExt.Round(min / 60d, 1);
            return hours;
        }
    }
}