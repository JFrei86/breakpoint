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

namespace breakpoint
{
    class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string && (string)parameter == "true")
            {
                if (value != null && value.GetType() == typeof(int) && (int)value == 0)
                {
                    return Visibility.Collapsed;
                }
                return (value == null) ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                if (value != null && value.GetType() == typeof(int) && (int)value == 0)
                {
                    return Visibility.Visible;
                }
                return (value == null) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
