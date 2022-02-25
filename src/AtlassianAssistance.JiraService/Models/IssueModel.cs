using System;
using System.Collections.Generic;
using System.Text;
using AtlassianAssistance.JiraService.Consts;
using AtlassianAssistance.JiraService.Contracts;

namespace AtlassianAssistance.JiraService.Models
{
    public class IssueModel : IssueBase
    {
        public string Reporter { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Assignee { get; set; }
        public string OriginalEstimate { get; set; }
        public string RemainingEstimate { get; set; }
        public string Status { get; set; }
        public string Resolution { get; set; }
        public DateTime? DueDate { get; set; }
        public IEnumerable<CommentModel> Comments { get; set; }
        public IEnumerable<AttachmentInfo> Attachments { get; set; }
        public LogWork LogWork { get; set; }
        public string[] FixVersions { get; set; }
        public string[] Labels { get; set; }
    }
}
