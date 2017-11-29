using System;
using System.Globalization;
using System.Windows.Data;

namespace AcadLib.WPF.Converters
{
    [ValueConversion(typeof(byte), typeof(double))]
    public class TransparenceInvertConverter : ConvertorBase
    {
        public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var transparence = (byte)value;
            return 255 - transparence;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bval = System.Convert.ToByte(value);
            return 255 - bval;
        }
    }
}
