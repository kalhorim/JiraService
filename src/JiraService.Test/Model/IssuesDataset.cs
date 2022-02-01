using System.Collections;
using System.Collections.Generic;
using JiraService.Test.Service;

namespace JiraService.Test.Model
{

    public class IssuesDataset : IEnumerable<object[]>
    {
        private IEnumerable<string> RegisterIssues => JiraService.Test.Service.Consts.GetSettingsModel().GetIssueKeys();

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