using System;
using System.Globalization;
using System.Windows.Data;

namespace AcadLib.WPF.Converters
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumDescriptionConverter : ConvertorBase
    {
        public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return value.Description();
        }
    }
}
