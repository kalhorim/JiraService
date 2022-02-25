using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtlassianAssistance.JiraService.Models
{
    public class JiraProject
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
    }
    public class JiraIssueType
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class JiraField
    {
        public JiraUser Reporter { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public JiraIssueType Issuetype { get; set; }
        public JiraProject Project { get; set; }


    }
    public class JiraIssue
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public JiraField Fields { get; set; }
    }
    public class JiraUser
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public bool Active { get; set; }

    }
    public class JiraWebhookCallbackResult
    {
        public string Timestamp { get; set; }
        public string WebhookEvent { get; set; }
        public string Issue_event_type_name { get; set; }
        public JiraUser User { get; set; }
        public JiraIssue Issue { get; set; }
    }

}