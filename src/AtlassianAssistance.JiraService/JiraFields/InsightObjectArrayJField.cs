using System;
using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.JiraFields
{
    public class InsightObjectArrayJField : JiraCustomFieldBase
    {
        public string[] Values { get; set; }
        protected internal override string GetJiraValue => Values == null ? null : string.Join(", ", Values);

        protected internal override string[] SetJiraValue { set => Values = value; }
        
    }
}
