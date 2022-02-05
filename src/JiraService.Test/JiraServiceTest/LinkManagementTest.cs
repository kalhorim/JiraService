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
    public class LinkManagementTest
    {
        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void GetLink_jiraService(IJiraService jiraService, string key)
        {
            var links = await jiraService.LinkManagement.GetIssueLinks(key);
            Assert.All(links, a =>
            {
                Assert.NotNull(a.LinkType);
                Assert.NotNull(a.InwardIssueKey);
                Assert.NotNull(a.OutwardIssueKey);
            });
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void GetLinks_jiraService(IJiraService jiraService, string key)
        {
            var issues = await jiraService.LinkManagement.GetLinks<IssueModel>(key, JiraLinkTypes.Cloners);
            Assert.All(issues, a =>
            {
                Assert.NotNull(a.Key);
                Assert.NotNull(a.Type);
                Assert.NotNull(a.ProjectKey);
            });
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void Link_jiraService(IJiraService jiraService, string key)
        {
            var newkey = await jiraService.Issue.Create(ChangeIssueInitializer.ChangeIssue.AddCustomFields());
            var exec = await jiraService.LinkManagement.Link(key, newkey, JiraLinkTypes.Duplicate);
            Assert.True(exec);
            await DeleteIssue(jiraService, newkey);
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