using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Models
{
    public class ChildIssue : IssueModel
    {
        public string ParentIssueKey { get; set; }
        public string LinkType { get; set; }
    }
}
