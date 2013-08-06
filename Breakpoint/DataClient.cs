using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using breakpoint.DataModel;
using Windows.UI.Popups;

namespace breakpoint
{
    class DataClient
    {
        public DataClient()
        {
            userOrgs = new ObservableCollection<User>();
            unreadNotifications = new ObservableCollection<Notification>();
            pinnedIssues = new ObservableCollection<Issue>();
            recentRepos = new ObservableCollection<Repository>();
        }
        private string accessToken;

        public DataType PageRootDataPointer;

        public User authenticatedUser { get { return _authenticatedUser; } set { _authenticatedUser = value; } }
        public ObservableCollection<User> userOrgs { get { return _userOrgs; } set { _userOrgs = value; } }

        public ObservableCollection<Notification> unreadNotifications { get { return _unreadNotifications; } set { _unreadNotifications = value; } }
        public ObservableCollection<Issue> pinnedIssues { get { return _pinnedIssues; } set { _pinnedIssues = value; }}
        public ObservableCollection<Repository> recentRepos { get { return _recentRepos; } set { _recentRepos = value; } }

        private User _authenticatedUser;
        private ObservableCollection<User> _userOrgs;
        private ObservableCollection<Notification> _unreadNotifications;
        private ObservableCollection<Issue> _pinnedIssues;
        private ObservableCollection<Repository> _recentRepos;

        public async void NotifyUser(String message)
        {
            await new MessageDialog(message).ShowAsync();
        }

        public async Task<String> authenticate()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader("breakpoint_secret");
            try
            {
                accessToken = await HttpUtil.GetAccessCode("https://github.com/login/oauth/", loader.GetString("ClientID"), loader.GetString("ClientSecret"), loader.GetString("Scope"));
            }
            catch (AccessTokenNotFoundException args)
            {
                NotifyUser("Uh oh, something went terribly wrong. Sorry about that! Breakpoint will now close. Verify network connectivity and try again. (" + args.Message + ")");
                App.Current.Exit();
            }
            return accessToken;
        }

        public async Task initializeData()
        {
            authenticatedUser = await HttpUtil.GetAuthenticatedUserAsync(accessToken);
            await updateUserOrganization();
            await updateNotifications();
        }
        public async Task initializeRepo(Repository repo, Dictionary<String, String> issueArgs)
        {
            await HttpUtil.GetRepoContentsAsync(accessToken, repo, issueArgs);
        }
        public async Task updateRepoIssues(Repository repo, Dictionary<String, String> args)
        {
            Issue[] issues = await HttpUtil.GetRepoIssuesAsync(accessToken, repo.url + "/issues", args);
            repo.issues.Clear();
            foreach (Issue a in issues)
            {
                repo.issues.Add(a);
            }
        }
        public async Task updateAssignee(User newAssignee, Issue issue)
        {
            issue.assignee = newAssignee;
            PostModel.Issue content = (PostModel.Issue)PostModel.PostType.ConvertToPostModel(issue);
            await HttpUtil.PostIssueAsync(accessToken, issue.url, content);
        }
        public async Task updateMilestone(Milestone newMilestone, Issue issue)
        {
            issue.milestone = newMilestone;
            PostModel.Issue content = (PostModel.Issue)PostModel.PostType.ConvertToPostModel(issue);
            await HttpUtil.PostIssueAsync(accessToken, issue.url, content);
        }
        public async Task getIssueContents(Issue issue)
        {
            await HttpUtil.GetIssueContentsAsync(accessToken, issue);
        }
        public async Task postIssueComment(Issue issue, String body)
        {
            PostModel.Comment comment = new PostModel.Comment();
            comment.body = body;
            Comment response = await HttpUtil.PostCommentAsync(accessToken, issue.url + "/comments", comment);
            issue.notes.Add(response);
            issue.comments++;
        }
        public DataType DataPointer { get { return PageRootDataPointer; } set { PageRootDataPointer = value; } }
        public async Task updateUserOrganization()
        {
            Task<User[]> orgs = HttpUtil.GetOrgsAsync(accessToken);
            Task<Repository[]> me_repos = HttpUtil.GetAuthUserReposAsync(accessToken);
            User[] me_orgs = await orgs;
            Task<Repository[]>[] org_repos = new Task<Repository[]>[me_orgs.Length];
            for (int i = 0; i < org_repos.Length; i++)
            {
                org_repos[i] = HttpUtil.GetReposAsync(accessToken, me_orgs[i].url + "/repos");
            }
            Repository[] temp_repo = await me_repos;
            authenticatedUser.repos = new ObservableCollection<Repository>();
            userOrgs.Clear();
            foreach (Repository repo in temp_repo)
            {
                repo.owner = authenticatedUser;
                authenticatedUser.repos.Add(repo);

            }
            userOrgs.Add(authenticatedUser);
            for (int i = 0; i < org_repos.Length; i++)
            {
                Repository[] repos = await org_repos[i];
                me_orgs[i].repos = new ObservableCollection<Repository>();
                foreach (Repository repo in repos)
                {
                    repo.owner = me_orgs[i];
                    me_orgs[i].repos.Add(repo);
                }
                userOrgs.Add(me_orgs[i]);
            }
        }
        public void PinIssue(Issue item)
        {
            pinnedIssues.Add(item);
        }
        public Issue UnpinIssue(Issue item)
        {
            if (pinnedIssues.Remove(item))
            {
                return item;
            }
            return null;
        }
        public void AddRecentRepository(Repository repo)
        {
            if(!recentRepos.Contains(repo))
                recentRepos.Insert(0, repo);
            else
            {
                recentRepos.Remove(repo);
                recentRepos.Insert(0, repo);
            }
            if (recentRepos.Count == 4)
            {
                recentRepos.RemoveAt(3);
            }
        }
        public async Task updateNotifications()
        {
            Notification[] response = await HttpUtil.GetNotifications(accessToken);
            unreadNotifications.Clear();
            foreach (Notification a in response)
            {
                unreadNotifications.Add(a);
            }
        }
        public async Task markNotificationsAsRead()
        {
            unreadNotifications.Clear();
            await HttpUtil.PutNotificationsAsync(accessToken);
        }
    }
}
