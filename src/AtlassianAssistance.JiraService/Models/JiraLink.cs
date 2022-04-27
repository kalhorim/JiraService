using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.Models
{
    public class JiraLink
    {
        public int LinkId { get; set; }
        public string InwardIssueKey { get; set; }
        public string OutwardIssueKey { get; set; }
        public JiraLinkType LinkType { get; set; }
    }
}
