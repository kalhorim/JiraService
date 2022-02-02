using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JiraService.Models;

namespace JiraService.Contracts
{
    public interface ICommentService
    {
        Task AddAttachmentsToIssue(string issueKey, IEnumerable<AttachmentInfo> attachmentInfos, CancellationToken token = default);
        Task AddCommentAsync(string issueKey, CommentModel comment, CancellationToken token = default);
        Task<IEnumerable<AttachmentInfo>> GetIssueAttachments(string issueKey, CancellationToken token = default);
        Task<IEnumerable<CommentModel>> GetIssueComments(string issueKey, CancellationToken token = default);
    }
}
