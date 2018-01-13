using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace AcadLib.WPF.Converters
{
    [Obsolete]
    [PublicAPI]
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumDescriptionConverter : ConvertorBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Description();
        }
    }
}