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
using breakpoint.DataModel;

namespace breakpoint
{
    class IssueNotesTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(Comment))
            {
                
                return Application.Current.Resources["CommentTemplate"] as DataTemplate;
            }
            else
            {
                return Application.Current.Resources["EventTemplate"] as DataTemplate;
            }
        }

    }
}
