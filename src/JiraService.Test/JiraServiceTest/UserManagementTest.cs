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

    }
}