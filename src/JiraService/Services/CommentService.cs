using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraService.Models;
using JiraService.Contracts;

namespace JiraService.Services
{
    internal class CommentService: ICommentService
    {
        private readonly Jira _jiraClient;
        public CommentService(Jira jiraClient)
        {
            _jiraClient = jiraClient;
        }

        #region Public Methods
        public async Task AddCommentAsync(string issueKey, CommentModel comment, CancellationToken token = default)
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey);

            await AddCommentAsync(issue, comment, token);
        }

        public async Task AddAttachmentsToIssue(string issueKey, IEnumerable<AttachmentInfo> attachmentInfos, CancellationToken token = default)
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey, token);
            await AddAttachmentsToIssue(issue, attachmentInfos);
        }

        public async Task<IEnumerable<AttachmentInfo>> GetIssueAttachments(string issueKey, CancellationToken token = default)
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey, token);
            var attachments = await issue.GetAttachmentsAsync();
            return attachments.Select(Mapper.Map);
        }

        public async Task<IEnumerable<CommentModel>> GetIssueComments(string issueKey, CancellationToken token = default)
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey);
            var comments = await issue.GetCommentsAsync();
            return comments.Select(Mapper.Map);
        }
        #endregion

        #region Private Methods
        internal async Task AddAttachmentsToIssue(Issue issue, IEnumerable<AttachmentInfo> attachmentInfos)
        {
            if (attachmentInfos == null)
                return;
            var attachments = attachmentInfos?.Select(x => new Atlassian.Jira.UploadAttachmentInfo(x.FileName, x.DataBytes))?.ToArray();
            if (attachments != null)
            {
                await issue.AddAttachmentAsync(attachments);
            }
            foreach (var comment in attachmentInfos)
            {

                var cmnt = new Atlassian.Jira.Comment()
                {
                    Author = comment.User.Username,
                    Body = comment.Body
                };
                await issue.AddCommentAsync(cmnt);
            }
        }
        private async Task AddCommentAsync(Issue issue, CommentModel comment, CancellationToken token)
        {
            await issue.AddCommentAsync(Mapper.Map(comment), token);
        }
        #endregion
    }
}
