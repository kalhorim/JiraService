using Atlassian.Jira;
using JiraService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraService.Models
{
    public class VersionValuesRequest : CustomFieldValuesRequest<string>
    {
        private readonly string projectKey;

        public VersionValuesRequest(string projectKey)
        {
            this.projectKey = projectKey;
        }

        public override async Task<IEnumerable<string>> GetValues(Jira jiraClient)
        {
            var versions = await jiraClient.Versions.GetVersionsAsync(projectKey);
            return versions.Select(x => x.Name);
        }
    }
}
