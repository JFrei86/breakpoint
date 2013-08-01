using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace breakpoint
{
    class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String color = (String)value;
            Windows.UI.Color rgb = new Windows.UI.Color();
            rgb.R = byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            rgb.G = byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            rgb.B = byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            rgb.A = 255;

            return new SolidColorBrush(rgb);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
