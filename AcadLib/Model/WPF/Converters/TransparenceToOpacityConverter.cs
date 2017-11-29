using System;
using System.Globalization;
using System.Windows.Data;

namespace AcadLib.WPF.Converters
{
    [ValueConversion(typeof(byte), typeof(double))]
    public class TransparenceToOpacityConverter : ConvertorBase
    {
        public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var transparence = (byte)value;
            var opacity = transparence / (double) byte.MaxValue;
            if (opacity < 0.1)
            {
                opacity = 0.1;
            }
            return opacity;
        }
    }
}
