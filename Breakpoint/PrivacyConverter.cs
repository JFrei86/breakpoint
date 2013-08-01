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
    /// <summary>
    /// Value converter that translates false to <see cref="Visibility.Visible"/> and true to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class PrivacyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? "Private" : "Public";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is String && (String)value == "Public";
        }
    }
}