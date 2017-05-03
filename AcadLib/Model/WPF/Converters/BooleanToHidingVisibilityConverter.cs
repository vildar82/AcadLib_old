using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AcadLib.WPF.Converters
{    
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToHidingVisibilityConverter : ConvertorBase
    {
        public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
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

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToOppositeVisibilityConverter : ConvertorBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return Visibility.Hidden;
            if ((bool)value)
            {
                return Visibility.Hidden;
            }
            return Visibility.Visible;
        }
    }
}
