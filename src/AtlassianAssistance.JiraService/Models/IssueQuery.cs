using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.Models
{
    public class IssueQueryRequest
    {
        public string Jql { get; set; }
        public int StartAt { get; set; }
        public int MaxIssuesPerRequest { get; set; }
    }

    public class IssueQueryResponse<T> where T : IssueModel
    {
        public IEnumerable<T> Result { get; internal set; }
        public int Total { get; internal set; }
    }
}
