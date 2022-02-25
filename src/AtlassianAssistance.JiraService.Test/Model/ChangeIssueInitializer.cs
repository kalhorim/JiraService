using System.Collections;
using System.Collections.Generic;
using AtlassianAssistance.JiraService.Test.Service;

namespace AtlassianAssistance.JiraService.Test.Model
{
    public class ChangeIssueInitializer : IEnumerable<object[]>
    {
        private IEnumerable<ChangeIssue> RegisterIssues
        {
            get
            {
                yield return ChangeIssue.AddTypeSafeCustomFields();
                //TODO: Add for loop status
                yield return ChangeIssue.AddTypeSafeCustomFields().AddAttachment().AddStatus().AddComment();

                yield return ChangeIssue.AddCustomFields();
                //TODO: Add for loop status
                yield return ChangeIssue.AddCustomFields().AddAttachment().AddStatus().AddComment();
            }
        }

        public static ChangeIssue ChangeIssue
        {
            get
            {
                var change = new ChangeIssue
                {
                    ProjectKey = "SRV",
                    Type = "Change",
                    Summary = "TEST",
                    Assignee = "jiraservice"
                };
                return change;
            }
        }

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