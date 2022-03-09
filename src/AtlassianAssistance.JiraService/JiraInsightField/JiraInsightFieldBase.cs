using AtlassianAssistance.JiraService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.JiraInsightField
{
    public abstract class JiraInsightFieldBase
    {
        protected internal abstract string[] GetInsightJiraValue { get; }
        //TODO:  for get data from server protected internal abstract string[] SetJiraValue { set; }
    }
}
