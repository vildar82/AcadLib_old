using System;
using System.Globalization;
using System.Windows.Data;

namespace AcadLib.WPF.Converters
{
    [ValueConversion(typeof(byte), typeof(double))]
    public class TransparenceToPercentConverter : ConvertorBase
    {
        public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var transparence = (byte)value;
            return (1 - transparence / (double) byte.MaxValue) * 100;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dval = System.Convert.ToDouble(value);
            return 100 - dval;
        }
    }
}
