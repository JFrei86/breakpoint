using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using breakpoint.Common;
using breakpoint.DataModel;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace breakpoint
{
    public sealed partial class FilterIssuesFlyout : SettingsFlyout
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        public FilterIssuesFlyout(Object context)
        {
            this.InitializeComponent();
            this.DataContext = context;
            this.Button_Click(null, null);
        }
        public FilterArgs generateArgs()
        {
            FilterArgs args = new FilterArgs(this);
            if (milestone.SelectedItem != null)
                args.Add("milestone", (milestone.SelectedItem as Milestone).number + "");
            else if (allIssuesCheckbox.IsChecked != null && (bool)allIssuesCheckbox.IsChecked)
                args.Add("milestone", "none");
            args.Add("state", (StateSwitch.IsOn) ? "open" : "closed");
            args.Add("direction", (OrderSwitch.IsOn) ? "asc" : "desc");
            string labels = "";
            foreach (Label a in LabelsList.SelectedItems)
            {
                labels += a.name + ",";
            }
            if (labels != "")
            {
                labels = labels.Substring(0, labels.Length - 1);
                args.Add("labels", labels);
            }
            if (SortCombo.SelectedItem != null)
                args.Add("sort", ((TextBlock)SortCombo.SelectedItem).Text.ToLower());
            else
                args.Add("sort", "created");
            return args;
        }
        public class FilterArgs {
            public FilterIssuesFlyout parent;
            public Dictionary<String, String> args = new Dictionary<string,string>();

            public FilterArgs(FilterIssuesFlyout _parent){
                parent = _parent;
            }
            public void Add(String value, String key)
            {
                args.Add(value, key);
            }
            public bool IsDefault()
            {
                if (parent == null)
                    return true;
                return !args.ContainsKey("milestone") && args["sort"] == "created" && args["direction"] == "desc" && !args.ContainsKey("labels");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            milestone.SelectedIndex = -1;
            StateSwitch.IsOn = true;
            OrderSwitch.IsOn = false;
            allIssuesCheckbox.IsChecked = false;
            SortCombo.SelectedIndex = 0;
            LabelsList.SelectedItems.Clear();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            milestone.SelectedIndex = -1;
        }
    }
}
