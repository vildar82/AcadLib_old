namespace AcadLib.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using JetBrains.Annotations;

    [Obsolete]
    [PublicAPI]
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToHidingVisibilityConverter : ConvertorBase
    {
        [NotNull]
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return Visibility.Collapsed;
            if ((bool)value)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
    }
}