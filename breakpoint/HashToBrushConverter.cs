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
    class HashToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Windows.UI.Color rgb = new Windows.UI.Color();
            Windows.UI.Color[] colorScheme = new Windows.UI.Color[5];
            colorScheme[0] = new Windows.UI.Color();
            for(int i = 0; i < colorScheme.Length; i++){
                colorScheme[i] = new Windows.UI.Color();
                colorScheme[i].A = 200;
            }
            colorScheme[0].R = 0;
            colorScheme[0].G = 75;
            colorScheme[0].B = 139;
            colorScheme[1].R = 3;
            colorScheme[1].G = 159;
            colorScheme[1].B = 223;
            colorScheme[2].R = 1;
            colorScheme[2].G = 91;
            colorScheme[2].B = 118;
            colorScheme[3].R = 129;
            colorScheme[3].G = 168;
            colorScheme[3].B = 9;
            colorScheme[4].R = 169;
            colorScheme[4].G = 218;
            colorScheme[4].B = 16;

            return new SolidColorBrush(colorScheme[value.GetHashCode() % 5]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}