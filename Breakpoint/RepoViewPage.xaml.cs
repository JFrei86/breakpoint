using breakpoint.Common;
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
using breakpoint.DataModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace breakpoint
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class RepoViewPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private DataClient client = (DataClient)Application.Current.Resources["client"];
        FilterIssuesFlyout.FilterArgs args;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public RepoViewPage()
        {
            this.InitializeComponent();
            if (!Application.Current.Resources.ContainsKey("CommentTemplate") && !Application.Current.Resources.ContainsKey("EventTemplate"))
            {
                Application.Current.Resources.Add(new KeyValuePair<object, object>("CommentTemplate", this.Resources["CommentTemplate"]));
                Application.Current.Resources.Add(new KeyValuePair<object, object>("EventTemplate", this.Resources["EventTemplate"])); 
            }
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.DefaultViewModel["repo"] = client.DataPointer;
            this.DefaultViewModel["FilterOn"] = false;
            this.DefaultViewModel["CommentBody"] = "";
            await client.initializeRepo((Repository)client.DataPointer, null);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void AppButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton b = sender as AppBarButton;
            if (b.Label == "Filter")
            {
                issuesList.SelectedIndex = -1;
                FilterIssuesFlyout flyout = new FilterIssuesFlyout(this.DefaultViewModel["repo"]);
                flyout.Unloaded += unloaded;
                flyout.ShowIndependent();
            }
            if (b.Label == "Clear Filter")
            {
                issuesList.SelectedIndex = -1;
                LoadingProgressBar.IsIndeterminate = true;
                this.DefaultViewModel["FilterOn"] = false;
                await client.updateRepoIssues(this.DefaultViewModel["repo"] as Repository, null);
                LoadingProgressBar.IsIndeterminate = false;
                args = null;
            }
            if (b.Label == "Home")
            {
                this.Frame.Navigate(typeof(HomePage));
            }
            if (b.Label == "Refresh")
            {
                int issue = issuesList.SelectedIndex;
                LoadingProgressBar.IsIndeterminate = true;
                await client.initializeRepo(this.DefaultViewModel["repo"] as Repository, (args == null) ? null : args.args);
                issuesList.SelectedIndex = issue;
                LoadingProgressBar.IsIndeterminate = false;
            }
            if (b.Label == "Pin")
            {
                if (issuesList.SelectedItem != null)
                {
                    client.PinIssue(issuesList.SelectedItem as Issue);
                    if(b.Flyout != null)
                        b.Flyout.ShowAt(b);
                }
            }
            if(b.Label == "Assign")
            {
                if (issuesList.SelectedItem != null && b.Flyout != null)
                {
                    b.Flyout.ShowAt(b);
                }
            }
        }
        public async void unloaded(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.IsIndeterminate = true;
            FilterIssuesFlyout.FilterArgs args = (sender as FilterIssuesFlyout).generateArgs();
            this.args = args;
            this.DefaultViewModel["FilterOn"] = !args.IsDefault();
            await client.updateRepoIssues(this.DefaultViewModel["repo"] as Repository, args.args);
            LoadingProgressBar.IsIndeterminate = false;
        }

        private async void newCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (issuesList.SelectedItem == null)
                return;
            LoadingProgressBar.IsIndeterminate = true;
            await client.postIssueComment(issuesList.SelectedItem as Issue, this.DefaultViewModel["CommentBody"] as string);
            this.DefaultViewModel["CommentBody"] = "";
            LoadingProgressBar.IsIndeterminate = false;
        }

        private async void issuesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((AppBarButton)(this.BottomAppBar as CommandBar).SecondaryCommands[0]).Visibility = (issuesList.SelectedItem == null) ? Visibility.Collapsed : Visibility.Visible;
            ((AppBarButton)(this.BottomAppBar as CommandBar).SecondaryCommands[1]).Visibility = (issuesList.SelectedItem == null) ? Visibility.Collapsed : Visibility.Visible;
            ((AppBarButton)(this.BottomAppBar as CommandBar).SecondaryCommands[2]).Visibility = (issuesList.SelectedItem == null) ? Visibility.Collapsed : Visibility.Visible;
            if (issuesList.SelectedItem == null)
                return;
            LoadingProgressBar.IsIndeterminate = true;
            await client.getIssueContents(issuesList.SelectedItem as Issue);
            LoadingProgressBar.IsIndeterminate = false;
        }

        private void newCommentBody_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.DefaultViewModel["CommentBody"] = (sender as TextBox).Text;
        }

        private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((sender as Button).Flyout != null)
            {
                (sender as Button).Flyout.ShowAt(sender as FrameworkElement);
            }

        }
        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (issuesList.SelectedItem != null && e.ClickedItem is User)
            {
                if ((sender as ListView).SelectedItem != null && (e.ClickedItem as User).url != (issuesList.SelectedItem as Issue).assignee.url)
                {
                    await client.updateAssignee(e.ClickedItem as User, issuesList.SelectedItem as Issue);
                }
                this.BottomAppBar.IsOpen = false;
            }
            if (issuesList.SelectedItem != null && e.ClickedItem is Milestone)
            {
                if (e.ClickedItem != null && (e.ClickedItem as Milestone).url != (issuesList.SelectedItem as Issue).milestone.url)
                {
                    await client.updateMilestone(e.ClickedItem as Milestone, issuesList.SelectedItem as Issue);
                }
                this.BottomAppBar.IsOpen = false;
            }
        }
    }
}
