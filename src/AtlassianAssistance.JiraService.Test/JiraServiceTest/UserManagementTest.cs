using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;
using AtlassianAssistance.JiraService.Test.Model;
using AtlassianAssistance.JiraService.Test.Service;
using AtlassianAssistance.JiraService.Consts;
using AtlassianAssistance.JiraService.Contracts;
using AtlassianAssistance.JiraService.Extensions;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using Xunit;

namespace AtlassianAssistance.JiraService.Test.JiraServiceTest
{
    public class UserManagementTest
    {
        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public void GetUsers_jiraService(IJiraService jiraService, string key)
        {
            //TODO Need write a valid query
            throw new Exception("please introduce GetUsers query ");

            //var links = await jiraService.UserManagement.GetUsers(key);
            //Assert.All(links, a => { Assert.NotNull(a.LinkType); Assert.NotNull(a.InwardIssueKey); Assert.NotNull(a.OutwardIssueKey); });
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async Task GetGroups_jiraService(IJiraService jiraService)
        {
            var maxResult = 2;
            var groups = await jiraService.UserManagement.GetGroups(maxResult);
            Assert.True(groups != null && groups.Count() <= maxResult);
        }
    }
}