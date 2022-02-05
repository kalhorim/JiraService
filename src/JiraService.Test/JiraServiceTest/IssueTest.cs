using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;
using JiraService.Test.Model;
using JiraService.Test.Service;
using JiraService.Consts;
using JiraService.Contracts;
using JiraService.Extensions;
using JiraService.JiraFields;
using JiraService.Models;
using Xunit;

namespace JiraService.Test.JiraServiceTest
{
    public class IssueTest
    {
        [Theory]
        [ClassData(typeof(ChangeIssueInitializer))]
        public async void CreateDeleteIssue_jiraService(IJiraService jiraService, ChangeIssue change)
        {
            var key = await jiraService.Issue.Create(change);
            Debug.WriteLine(key);
            Assert.True(!string.IsNullOrEmpty(key));
            var newBorn = await jiraService.Issue.Get<ChangeIssue>(key);
            Assert.True(newBorn.Equals(change));
            await DeleteIssue(jiraService, key);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void UpdateIssue_jiraService(IJiraService jiraService)
        {
            var summary = " Update";
            var change = ChangeIssueInitializer.ChangeIssue.AddCustomFields();
            await jiraService.Issue.Create(change);
            change = await jiraService.Issue.Get<ChangeIssue>(change.Key);
            change.Summary += summary;
            change.UpdateCustomFields();
            var update = await jiraService.Issue.Update(change);
            Assert.True(update);
            change = await jiraService.Issue.Get<ChangeIssue>(change.Key);
            Assert.Contains(summary, change.Summary);
            Assert.True(change.IsUpdated());
            await DeleteIssue(jiraService, change.Key);
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void GetIssue_jiraService(IJiraService jiraService, string key)
        {
            var issue = await jiraService.Issue.Get<ChangeIssue>(key);
            Assert.NotNull(issue);
            Assert.NotNull(issue.ChangeType);
            Assert.Equal(key, issue.Key);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public void GetIssues_jiraService(IJiraService jiraService)
        {
            var issues = jiraService.Issue.Query<ChangeIssue>("assignee in (membersOf(TDO))");
            Assert.All(issues, a =>
            {
                Assert.NotNull(a.Key);
                Assert.NotNull(a.Type);
                Assert.NotNull(a.ProjectKey);
            });
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void AddTrackingTime_jiraService(IJiraService jiraService, string key)
        {
            throw new Exception("please introduce AddTrackingTime process ");
            //var links = await jiraService.AddTrackingTime(key);
            //Assert.All(links, a => { Assert.NotNull(a.LinkType); Assert.NotNull(a.InwardIssueKey); Assert.NotNull(a.OutwardIssueKey); });
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void ChangeStatus_jiraService(IJiraService jiraService, string key)
        {
            await jiraService.Issue.ChangeStatus(key, "Planning");
            var issue = await jiraService.Issue.Get<IssueModel>(key);
            Assert.Equal("Planning", issue.Status);
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void GetInsightJField_CheckValue_jiraService(IJiraService jiraService, string key)
        {
            var issue = await jiraService.Issue.Get<ChangeIssue>(key);
            Assert.True(issue.Customer != null);
            Assert.True(issue.Customer.Value.Id != null);
            Assert.True(issue.Customer.Value.Key != null);
            Assert.True(issue.Customer.Value.Name != null);
        }

        private static async Task DeleteIssue(IJiraService jiraService, string key)
        {
            await jiraService.Issue.Delete(key);
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(
                async () => await jiraService.Issue.Get<ChangeIssue>(key));
            Assert.Contains("Issue Does Not Exist", exception.Message);
        }
    }
}