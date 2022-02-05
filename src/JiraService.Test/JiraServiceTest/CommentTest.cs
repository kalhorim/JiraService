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
    public class CommentTest
    {
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
            var comment = new CommentModel {Body = "Test", User = new User("Kalhori")};
            await jiraService.Comment.AddCommentAsync(key, comment);
            var comments = await jiraService.Comment.GetIssueComments(key);
            Assert.Contains(comments, a => a.Body.Equals(comment.Body));
        }

    }
}