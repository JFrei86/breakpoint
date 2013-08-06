using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Foundation.Collections;

namespace breakpoint.DataModel
{   
    [DataContract]
    class DataType : BindableBase
    {
        public static Type[] model = { typeof(DataType), typeof(Organization), typeof(User), typeof(Milestone), 
                                         typeof(Label), typeof(Issue), typeof(Comment), typeof(Repository), typeof(Event),
                                     typeof(DataType[]), typeof(Organization[]), typeof(User[]), typeof(Milestone[]), 
                                         typeof(Label[]), typeof(Issue[]), typeof(Comment[]), typeof(Repository[]), typeof(Event[]),
                                     typeof(Notification), typeof(Notification[])};

        public bool isComplete { get { return _complete; } set { this.SetProperty(ref this._complete, value); } }
        
        [DataMember]
        public String url { get { return _url; } set { this.SetProperty(ref this._url, value); } }
        [DataMember]
        public int id { get { return _id; } set { this.SetProperty(ref this._id, value); } }
        [DataMember]
        public int number { get { return _number; } set { this.SetProperty(ref this._number, value); } }
        [DataMember]
        public String created_at { get { return _created_at; } set { this.SetProperty(ref this._created_at, value); } }

        public override bool Equals(object Other)
        {
            return Other is DataType && ((DataType)Other).url == url;
        }

        private bool _complete;
        private String _url;
        private int _id;
        private int _number;
        private String _created_at;
    }
    [DataContract]
    class Organization : User
    {
    }
    [DataContract]
    class Repository : DataType
    {
        [DataMember]
        public String full_name { get { return _full_name; } set { this.SetProperty(ref this._full_name, value); } }
        [DataMember]
        public String name { get { return _name; } set {  this.SetProperty(ref this._name, value); } }
        [DataMember]
        public String description { get { return _description; } set {  this.SetProperty(ref this._description, value); } }

        public User owner { get { return _owner; } set {  this.SetProperty(ref this._owner, value); } }
        [DataMember(Name = "private")]
        public bool isPrivate { get { return _private; } set {  this.SetProperty(ref this._private, value); } }
        [DataMember]
        public int forks { get { return _forks; } set {  this.SetProperty(ref this._forks, value); } }
        [DataMember]
        public int watchers { get { return _watchers; } set { this.SetProperty(ref this._watchers, value); } }
        [DataMember]
        public int open_issues { get { return _open_issues; } set { this.SetProperty(ref this._open_issues, value); } }
        [DataMember]
        public String updated_at { get { return _updated_at; } set { this.SetProperty(ref this._updated_at, value); } }
        [DataMember]
        public String pushed_at { get { return _pushed_at; } set { this.SetProperty(ref this._pushed_at, value); } }

        public ObservableCollection<Issue> issues { get { return _issues; } set { this.SetProperty(ref this._issues, value); } }
        public ObservableCollection<Label> labels { get { return _labels; } set { this.SetProperty(ref this._labels, value); } }
        public ObservableCollection<Milestone> milestones { get { return _milestones; } set { this.SetProperty(ref this._milestones, value); } }
        public ObservableCollection<User> people { get { return _people; } set { this.SetProperty(ref this._people, value); } }
        public Milestone active_milestone { get; set; }

        private String _full_name;
        private String _name;
        private String _description;
        private User _owner;
        private bool _private;
        private int _forks;
        private int _watchers;
        private int _open_issues;
        private String _updated_at;
        private String _pushed_at;
        private ObservableCollection<Issue> _issues;
        private ObservableCollection<Label> _labels;
        private ObservableCollection<Milestone> _milestones;
        private ObservableCollection<User> _people;

    }
    [DataContract]
    class User : DataType
    {
        public User()
        {
            _repos = new ObservableCollection<Repository>();
        }
        [DataMember]
        public String login { get { return _login; } set { this.SetProperty(ref this._login, value); } }
        [DataMember]
        public String avatar_url { get { return _avatar_url; } set { this.SetProperty(ref this._avatar_url, value); } }
        [DataMember]
        public String name { get { return _name; } set { this.SetProperty(ref this._name, value); } }

        public ObservableCollection<Repository> repos { get { return _repos; } set { this.SetProperty(ref this._repos, value); } }

        private ObservableCollection<Repository> _repos;
        private String _login;
        private String _avatar_url;
        private String _name;

        [DataMember]
        public String company { get { return _company; } set { this.SetProperty(ref this._company, value); } }
        [DataMember]
        public String location { get { return _location; } set { this.SetProperty(ref this._location, value); } }
        [DataMember]
        public int public_repos { get { return _public_repos; } set { this.SetProperty(ref this._public_repos, value); } }
        [DataMember]
        public int followers { get { return _followers; } set { this.SetProperty(ref this._followers, value); } }
        [DataMember]
        public int following { get { return _following; } set { this.SetProperty(ref this._following, value); } }

        private String _company;
        private String _location;
        private int _public_repos;
        private int _followers;
        private int _following;

    }
    [DataContract]
    class Milestone : DataType
    {
        [DataMember]
        public String state { get { return _state; } set { this.SetProperty(ref this._state, value); } }
        [DataMember]
        public String title { get { return _title; } set { this.SetProperty(ref this._title, value); } }
        [DataMember]
        public String description { get { return _description; } set { this.SetProperty(ref this._description, value); } }
        [DataMember]
        public int open_issues { get { return _open_issues; } set { this.SetProperty(ref this._open_issues, value); } }
        [DataMember]
        public int closed_issues { get { return _closed_issues; } set { this.SetProperty(ref this._closed_issues, value); } }
        [DataMember]
        public String due_on { get { return _due_on; } set { this.SetProperty(ref this._due_on, value); } }

        public int issue_count { get { return open_issues + closed_issues; } }

        public override string ToString()
        {
            return title;
        }

        private String _state;
        private String _title;
        private String _description;
        private int _open_issues;
        private int _closed_issues;
        private String _due_on;
    }
    [DataContract]
    class Issue : DataType
    {
        [DataMember]
        public String state { get{return _state;} set{ this.SetProperty(ref this._state, value);} }
        [DataMember]
        public String title { get{return _title;} set{ this.SetProperty(ref this._title, value);} }
        [DataMember]
        public String body { get{return _body;} set{ this.SetProperty(ref this._body, value);} }
        [DataMember]
        public User user { get{return _owner;} set{ this.SetProperty(ref this._owner, value);} }
        [DataMember]
        public Label[] labels { get{return _labels;} set{ this.SetProperty(ref this._labels, value);} }
        [DataMember]
        public User assignee { get{return _assignee;} set{ this.SetProperty(ref this._assignee, value);} }
        [DataMember]
        public Milestone milestone { get{return _milestone;} set{ this.SetProperty(ref this._milestone, value);} }
        [DataMember]
        public int comments { get{return _comments;} set{ this.SetProperty(ref this._comments, value);} }
        [DataMember]
        public String closed_at { get{return _closed_at;} set{ this.SetProperty(ref this._closed_at, value);} }
        [DataMember]
        public String updated_at { get{return _updated_at;} set{ this.SetProperty(ref this._updated_at, value);} }

        public ObservableCollection<DataType> notes { get { return _notes; } set { this.SetProperty(ref this._notes, value); } }

        private ObservableCollection<DataType> _notes;

        private String _state;
        private String _title;
        private String _body;
        private User _owner;
        private Label[] _labels;
        private User _assignee;
        private Milestone _milestone;
        private int _comments;
        private String _closed_at;
        private String _updated_at;
    }
    [DataContract]
    class Label : DataType
    {
        [DataMember]
        public String name { get { return _name; } set { this.SetProperty(ref this._name, value); } }
        [DataMember]
        public String color { get { return _color; } set { this.SetProperty(ref this._color, value); } }

        private String _name;
        private String _color;
    }
    [DataContract]
    class Comment : DataType
    {
        [DataMember]
        public String body { get { return _body; } set { this.SetProperty(ref this._body, value); } }
        [DataMember]
        public User user { get { return _user; } set { this.SetProperty(ref this._user, value); } }

        private String _body;
        private User _user;
    }
    [DataContract]
    class Event : DataType
    {
        [DataMember(Name = "event")]
        public String verb { get { return _event; } set { this.SetProperty(ref this._event, value); } }
        [DataMember]
        public User actor { get { return _actor; } set { this.SetProperty(ref this._actor, value); } }

        private String _event;
        private User _actor;
    }
    [DataContract]
    class Notification : DataType
    {
        private Issue _subject;

        [DataMember]
        public Issue subject
        {
            get { return _subject; }
            set { this.SetProperty(ref this._subject, value); }
        }
    }
}
