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

namespace JiraService.Test
{

    public class IntegrationTest
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
        public async void GetIssueAttachments_jiraService(IJiraService jiraService, string key)
        {
            var attachments = await jiraService.Comment.GetIssueAttachments(key);
            Assert.All(attachments, a =>
            {
                Assert.NotNull(a.User.Username);
                Assert.NotNull(a.DataBytes);
            });
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void GetIssueComments_jiraService(IJiraService jiraService, string key)
        {
            var comments = await jiraService.Comment.GetIssueComments(key);
            Assert.All(comments, a =>
            {
                Assert.NotNull(a.Body);
                Assert.NotNull(a.User.Username);
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
        public void GetUsers_jiraService(IJiraService jiraService, string key)
        {
            //TODO Need write a valid query
            throw new Exception("please introduce GetUsers query ");

            //var links = await jiraService.UserManagement.GetUsers(key);
            //Assert.All(links, a => { Assert.NotNull(a.LinkType); Assert.NotNull(a.InwardIssueKey); Assert.NotNull(a.OutwardIssueKey); });
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

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void AddAttachmentsToIssue_jiraService(IJiraService jiraService, string key)
        {
            var attachments = new List<AttachmentInfo>
            {
                IssueFakeExtentions.TextFileAttachment()
            };
            await jiraService.Comment.AddAttachmentsToIssue(key, attachments);
            var newAttachment = await jiraService.Comment.GetIssueAttachments(key);
            Assert.Contains(newAttachment, a => attachments[0].FileName.Equals(a.FileName));
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void AddCommentAsync_jiraService(IJiraService jiraService, string key)
        {
            var comment = new CommentModel {Body = "Test", User = new User("Kalahari")};
            await jiraService.Comment.AddCommentAsync(key, comment);
            var comments = await jiraService.Comment.GetIssueComments(key);
            Assert.Contains(comments, a => a.Body.Equals(comment.Body));
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
        public async void GetCustomFields_jiraService(IJiraService jiraService, string key)
        {
            var cfs = await jiraService.Schema.GetCustomFields();
            Assert.True(cfs.Any());
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_GetKeyOfValueInInsightField_Compare_jiraService(
            IJiraService jiraService)
        {
            var bservices =
                await jiraService.Schema.GetInsightFieldValues(
                    IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), 0, 100);
            Assert.NotEmpty(bservices);
            var firstbs = bservices.FirstOrDefault();
            var insight =
                await jiraService.Schema.GetKeyOfValueInInsightField(
                    IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), firstbs.Name);
            Assert.Equal(firstbs, insight);
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

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_jiraService(IJiraService jiraService)
        {
            var change = new ChangeIssue();
            change.BusinessService = new InsightJField
            {
                Value = (await jiraService.Schema.GetInsightFieldValues(change.GetFieldId(f => f.BusinessService), 0, 100))
                    .LastOrDefault()
            };
            change.Customer = new InsightJField
                {Value = new InsightField {Id = 265, Key = "CRM-265", Name = "Test data"}};
            change.SetAttributeInCustomFields();
            await change.GetCustomFields().Where(w => w.Value is InsightJField).Select(s => (InsightJField) s.Value)
                .ToList().ForEachAsync(async customField =>
                {
                    var value = await jiraService.Schema.GetKeyOfValueInInsightField(customField.Attribute.FieldId,
                        customField.Value.Name);
                    Assert.Equal(value.Key, customField.Value.Key);
                });
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