using AtlassianAssistance.JiraService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.JiraInsightField
{
    public abstract class JiraInsightFieldBase
    {
        protected internal abstract string[] GetInsightJiraValue { get; }
        protected internal abstract void SetJiraValue(IEnumerable<object> value);
    }
}
