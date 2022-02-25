using System.Collections;
using System.Collections.Generic;
using AtlassianAssistance.JiraService.Test.Service;

namespace AtlassianAssistance.JiraService.Test.Model
{

    public class IssuesDataset : IEnumerable<object[]>
    {
        private IEnumerable<string> RegisterIssues => AtlassianAssistance.JiraService.Test.Service.Consts.GetSettingsModel().GetIssueKeys();

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var provider in new JiraServiceProvider())
            foreach (var issue in RegisterIssues)
                yield return new[] {provider[0], issue};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}