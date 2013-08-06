using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Data.Json;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net.Http;
using System.Collections.ObjectModel;
using breakpoint.DataModel;
using breakpoint.PostModel;
using Windows.UI.Popups;
namespace breakpoint
{
    public class AccessTokenNotFoundException : System.Exception
    {
        public AccessTokenNotFoundException() { }
        public AccessTokenNotFoundException(String message) { }
        public AccessTokenNotFoundException(String message, System.Exception inner) { }

    }
    public class DataRetrievalException : System.InvalidOperationException
    {
        public DataRetrievalException() { }
        public DataRetrievalException(String message) { }
        public DataRetrievalException(String message, System.Exception inner) { }
    }
    static class HttpUtil
    {
        public static string baseUri = "https://api.github.com/";
        public static async Task<String> GetAccessCode(String OAuthURI, String clientid, String secret, String scope)
        {
            try
            {
                WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(OAuthURI + "authorize?client_id=" + clientid + "&scope=" + scope + "&request_type=token"));
                if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                    throw new AccessTokenNotFoundException(result.ResponseErrorDetail.ToString());
                if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
                    throw new AccessTokenNotFoundException("USER CANCELLED");
                else
                {
                    String[] rawResponseData = result.ResponseData.ToString().Split('?', '=', '&');
                    String code = "";
                    for (int i = 0; i < rawResponseData.Length; i++)
                    {
                        if (rawResponseData[i] == "code")
                        {
                            code = rawResponseData[i + 1];
                            break;
                        }
                    }
                    if (code == "")
                        throw new AccessTokenNotFoundException("ERROR: CODE NOT FOUND");
                    HttpResponseMessage message = await GetRawAsync(OAuthURI + "access_token?client_id=" + clientid + "&client_secret=" + secret + "&code=" + code);
                    String[] rawAccessData = (await message.Content.ReadAsStringAsync()).Split('?', '=', '&');
                    for (int i = 0; i < rawAccessData.Length; i++)
                    {
                        if (rawAccessData[i] == "access_token")
                        {
                            return rawAccessData[i + 1];
                        }
                    }
                    throw new AccessTokenNotFoundException("ERROR: ACCESS TOKEN NOT FOUND");
                }

            }
            catch (System.IO.FileNotFoundException exception)
            {
                throw new AccessTokenNotFoundException("ERROR: UNKNOWN (Check Connection)", exception);
            }
        }
        public static async Task<Object> ParseResponse(HttpResponseMessage response, Type parseType)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(parseType, DataType.model);
            Stream content = await response.Content.ReadAsStreamAsync();
            content.Position = 0;
            return ser.ReadObject(content);
        }
        public static async Task<HttpResponseMessage> GetRawAsync(String uri)
        {
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "WinRT-Breakpoint");
            return await http.GetAsync(uri);
        }
        public static async Task<HttpResponseMessage> PutAsync(String access_code, String uri)
        {
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "WinRT-Breakpoint");
            http.DefaultRequestHeaders.Add("Authorization", "token " + access_code);
            MemoryStream ms = new MemoryStream();
            ms.Position = 0;
            HttpContent message = new StreamContent(ms);

            HttpResponseMessage response = await http.PutAsync(uri, message);
            if (!response.IsSuccessStatusCode)
            {
                throw new DataRetrievalException(await response.Content.ReadAsStringAsync());
            }
            return response;
        }
        public static async Task<HttpResponseMessage> PutNotificationsAsync(String access_code)
        {
            return await PutAsync(access_code, baseUri + "notifications");
        }
        public static async Task<Object> GetAsync(String access_code, String Uri, Dictionary<String,String> args, Type parseType)
        {
            if (args != null && args.Count != 0)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    if (i == 0)
                        Uri += "?" + args.ElementAt(i).Key + "=" + args.ElementAt(i).Value;
                    else
                        Uri += "&" + args.ElementAt(i).Key + "=" + args.ElementAt(i).Value;
                }
            }
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "WinRT-Breakpoint");
            http.DefaultRequestHeaders.Add("Authorization", "token " + access_code);

            HttpResponseMessage response = await http.GetAsync(Uri);
            if (!response.IsSuccessStatusCode)
            {
                throw new DataRetrievalException(await response.Content.ReadAsStringAsync());
            }
            if (parseType == null)
                return response;
            return await ParseResponse(response, parseType);
        }
        public static async Task<User> GetAuthenticatedUserAsync(String access_code)
        {
            String uri = baseUri + "user";
            Object response = await GetAsync(access_code, uri, null, typeof(DataModel.User));
            return (User)response;
        }
        public static async Task<Repository[]> GetAuthUserReposAsync(String access_code)
        {
            String uri = baseUri + "user/repos";
            Object response = await GetAsync(access_code, uri, null, typeof(Repository[]));
            return (Repository[])response;
        }
        public static async Task<Repository[]> GetReposAsync(String access_code, String Uri)
        {
            Object response = await GetAsync(access_code, Uri, null, typeof(Repository[]));
            return (Repository[])response;
        }
        public static async Task<User[]> GetOrgsAsync(String access_code)
        {
            String uri = baseUri + "user/orgs";
            Object response = await GetAsync(access_code, uri, null, typeof(User[]));
            User[] orgs = (User[])response;
            for (int i = 0; i < orgs.Length; i++ )
            {
                orgs[i] = (User)await GetAsync(access_code, orgs[i].url, null, typeof(User));
            }
            return (User[])response;
        }
        public static async Task<Notification[]> GetNotifications(String access_code)
        {
            Object response = await GetAsync(access_code, baseUri + "notifications", null, typeof(Notification[]));
            return (Notification[])response;
        }
        public static async Task<Milestone[]> GetRepoMilestonesAsync(String access_code, String Uri)
        {
            Object response = await GetAsync(access_code, Uri, null, typeof(Milestone[]));
            return (Milestone[])response;
        }
        public static async Task<DataModel.Issue[]> GetRepoIssuesAsync(String access_code, String Uri, Dictionary<String,String> args)
        {
            Object response = await GetAsync(access_code, Uri, args, typeof(DataModel.Issue[]));
            return (DataModel.Issue[])response;
        }
        public static async Task<DataModel.User[]> GetRepoPeopleAsync(String access_code, String Uri)
        {
            Object response = await GetAsync(access_code, Uri, null, typeof(User[]));
            return (DataModel.User[])response;
        }
        public static async Task<Label[]> GetRepoLabelsAsync(String access_code, String Uri)
        {
            Object response = await GetAsync(access_code, Uri, null, typeof(DataModel.Label[]));
            return (DataModel.Label[])response;
        }
        public static async Task GetRepoContentsAsync(String access_code, Repository repo, Dictionary<String, String> args)
        {
            Task<DataModel.Issue[]> task_issues = HttpUtil.GetRepoIssuesAsync(access_code, repo.url + "/issues", args);
            Task<Milestone[]> task_mil = HttpUtil.GetRepoMilestonesAsync(access_code, repo.url + "/milestones");
            Task<Label[]> task_labels = HttpUtil.GetRepoLabelsAsync(access_code, repo.url + "/labels");
            Task<User[]> task_collab = HttpUtil.GetRepoPeopleAsync(access_code, repo.url + "/assignees");

            repo.labels = new ObservableCollection<Label>();
            Label[] labels = await task_labels;
            foreach (Label a in labels)
            {
                repo.labels.Add(a);
            }

            repo.milestones = new ObservableCollection<Milestone>();
            Milestone[] milestones = await task_mil;
            foreach (Milestone a in milestones)
            {
                repo.milestones.Add(a);
            }

            repo.issues = new ObservableCollection<DataModel.Issue>();
            DataModel.Issue[] issues = await task_issues;
            foreach (DataModel.Issue a in issues)
            {
                repo.issues.Add(a);
            }

            repo.people = new ObservableCollection<User>();
            User[] people = await task_collab;
            foreach (User a in people)
            {
                repo.people.Add(a);
            }
        }
        public static async Task<DataModel.Comment[]> GetIssueCommentsAsync(String access_code, String Uri)
        {
            Object response = await GetAsync(access_code, Uri, null , typeof(DataModel.Comment[]));
            return (DataModel.Comment[])response;
        }
        public static async Task<Event[]> GetIssueEventsAsync(String access_code, String Uri)
        {
            Object response = await GetAsync(access_code, Uri, null , typeof(DataModel.Event[]));
            return (DataModel.Event[])response;
        }
        public static async Task GetIssueContentsAsync(String access_code, DataModel.Issue issue)
        {
            Task<Event[]> taskA = GetIssueEventsAsync(access_code, issue.url + "/events");
            Task<DataModel.Comment[]> taskB = GetIssueCommentsAsync(access_code, issue.url + "/comments?sort=created&direction=desc");
            Event[] events = await taskA;
            DataModel.Comment[] comments = await taskB;
            if(events.Length + comments.Length != 0){
                issue.notes = new ObservableCollection<DataType>();
                int i = events.Length - 1;
                int j = comments.Length - 1;
                while (i > -1 || j > -1)
                {
                   if (i > -1 && j > -1 && TimeUtil.stringToDate(events[i].created_at) <= TimeUtil.stringToDate(comments[j].created_at))
                   {
                       issue.notes.Add(events[i]);
                       i--;
                   }
                   else if (j > -1)
                   {
                       issue.notes.Add(comments[j]);
                       j--;
                   }
                   else if(i > -1)
                   {
                       issue.notes.Add(events[i]);
                       i--;
                   }
                } 
            }
        }
        public static async Task<Object> PostAsync(String access_code, String Uri, PostType content, Type parseType)
        {
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "WinRT-Breakpoint");
            http.DefaultRequestHeaders.Add("Authorization", "token " + access_code);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(content.GetType());

            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, content);
            ms.Position = 0;
            HttpContent message = new StreamContent(ms);

            HttpResponseMessage response = await http.PostAsync(Uri, message);
            if (!response.IsSuccessStatusCode)
            {
                throw new DataRetrievalException(await response.Content.ReadAsStringAsync());
            }
            return await ParseResponse(response, parseType);
        }
        public static async Task<DataModel.Comment> PostCommentAsync(String access_code, String Uri, PostModel.Comment comment)
        {
            Object message = await PostAsync(access_code, Uri, comment, typeof(DataModel.Comment));
            return (DataModel.Comment)message;
        }
        public static async Task<DataModel.Issue> PostIssueAsync(String access_code, String Uri, PostModel.Issue issue)
        {
            Object message = await PostAsync(access_code, Uri, issue, typeof(DataModel.Issue));
            return (DataModel.Issue)message;
        }
    }
}
