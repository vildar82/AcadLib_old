﻿using System;
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
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    [ValueConversion(typeof(System.Drawing.Color), typeof(SolidColorBrush))]    
    [ValueConversion(typeof(Autodesk.AutoCAD.Colors.Color), typeof(SolidColorBrush))]
    public class ColorToBrushConverter : ConvertorBase
    {
        public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color)
            {
                var dc = (System.Drawing.Color)value;
                return new SolidColorBrush(Color.FromArgb(dc.A, dc.R, dc.G, dc.B));
            }

            if (value is Color)
            {
                return new SolidColorBrush((Color)value);
            }

            if (value is Autodesk.AutoCAD.Colors.Color)
            {
                var ac = (Autodesk.AutoCAD.Colors.Color)value;
                return new SolidColorBrush(Color.FromRgb(ac.Red, ac.Green, ac.Blue));
            }

            return null;
        }
    }
}