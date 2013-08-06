using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Windows.Foundation.Collections;
using breakpoint.DataModel;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace breakpoint.PostModel
{
    [DataContract]
    class Comment : PostType
    {
        [DataMember]
        public string body { get { return _body; } set { this.SetProperty(ref this._body, value); } }
        private string _body;
    }
    [DataContract]
    class Issue : PostType
    {
        [DataMember]
        public string title { get { return _title; } set { this.SetProperty(ref this._title, value); } }
        [DataMember]
        public string body { get { return _body; } set { this.SetProperty(ref this._body, value); } }
        [DataMember]
        public string assignee { get { return _assignee; } set { this.SetProperty(ref this._assignee, value); } }
        [DataMember]
        public int milestone { get { return _milestone; } set { this.SetProperty(ref this._milestone, value); } }
        [DataMember]
        public string state { get { return _state; } set { this.SetProperty(ref this._state, value); } }
        [DataMember]
        public string[] labels { get { return _labels; } set { this.SetProperty(ref this._labels, value); } }

        private string _body;
        private string _title;
        private string _assignee;
        private int _milestone;
        private string _state;
        private string[] _labels;
    }
    [DataContract]
    class PostType : BindableBase
    {
        public static PostType ConvertToPostModel(DataType content)
        {
            if (typeof(DataModel.Issue) == content.GetType())
            {
                DataModel.Issue typedContent = (DataModel.Issue)content;
                Issue rtn = new Issue();
                rtn.title = typedContent.title;
                rtn.body = typedContent.body;
                if (typedContent.assignee != null)
                    rtn.assignee = typedContent.assignee.login;
                rtn.labels = new string[typedContent.labels.Length];
                for (int i = 0; i < rtn.labels.Length; i++)
                {
                    rtn.labels[i] = typedContent.labels[i].name;
                }
                rtn.state = typedContent.state;
                if(typedContent.milestone != null)
                    rtn.milestone = typedContent.milestone.number;
                return rtn;
            }
            if (typeof(DataModel.Comment) == content.GetType())
            {
                DataModel.Comment typedContent = (DataModel.Comment)content;
                Comment rtn = new Comment();
                rtn.body = typedContent.body;
                return rtn;
            }
            else
            {
                return null;
            }
        }
    }
}
