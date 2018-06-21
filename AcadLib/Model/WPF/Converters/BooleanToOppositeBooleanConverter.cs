namespace AcadLib.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using JetBrains.Annotations;

    [Obsolete]
    [PublicAPI]
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanToOppositeBooleanConverter : ConvertorBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }

            return false;
        }
    }
}