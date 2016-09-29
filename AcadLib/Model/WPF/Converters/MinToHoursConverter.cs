using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace AcadLib.WPF.Converters
{
    public class MinToHoursConverter : MarkupExtension, IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var min = System.Convert.ToInt32(value);
            var hours = Math.Round(min / 60d, 1);
            return hours;
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue (IServiceProvider serviceProvider)
        {
            if (_converter == null)
                _converter = new MinToHoursConverter();
            return _converter;
        }
        private static MinToHoursConverter _converter = null;
    }
}
