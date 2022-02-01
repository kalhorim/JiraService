using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Models
{
    public class JiraLink
    {
        public string InwardIssueKey { get; set; }
        public string OutwardIssueKey { get; set; }
        public JiraLinkType LinkType { get; set; }
    }
}
