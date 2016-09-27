using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace AcadLib.WPF.Converters
{
    public class ColorToBrushConverter : MarkupExtension, IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dc = (System.Drawing.Color)value;
            return new SolidColorBrush(new Color { A = dc.A, R = dc.R, G = dc.G, B = dc.B });
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue (IServiceProvider serviceProvider)
        {
            if (_converter == null)
                _converter = new ColorToBrushConverter();
            return _converter;
        }
        private static ColorToBrushConverter _converter = null;
    }
}
