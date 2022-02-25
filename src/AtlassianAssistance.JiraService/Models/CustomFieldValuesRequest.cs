using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianAssistance.JiraService.Models
{
    public abstract class CustomFieldValuesRequest<T>
    {
        public abstract Task<IEnumerable<T>> GetValues(Jira jiraClient);
    }
}
