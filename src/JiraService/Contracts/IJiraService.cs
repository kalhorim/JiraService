using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JiraService.Consts;
using JiraService.Models;

namespace JiraService.Contracts
{
    public interface IJiraService
    {
        Task AddAttachmentsToIssue(string issueKey, IEnumerable<AttachmentInfo> attachmentInfos, CancellationToken token = default);
        Task AddCommentAsync(string issueKey, CommentModel comment, CancellationToken token = default);
        Task AddTrackingTime(string issueKey, string originalEstimate, string remainingEstimate, CancellationToken token = default);
        Task ChangeStatus(string issueKey, string newStatus, string newResolution = null, CancellationToken token = default);
        Task<string> CreateIssue(IssueModel IssueModel, string parentIssueKey = null, CancellationToken token = default);
        Task<IEnumerable<CustomField>> GetCustomFields();
        Task<IEnumerable<HistoryField>> GetHistory(string issueKey, string fieldName, CancellationToken token = default);
        Task<T> GetIssue<T>(string issueKey, CancellationToken token = default) where T : IssueModel;
        Task<IEnumerable<AttachmentInfo>> GetIssueAttachments(string issueKey, CancellationToken token = default);
        Task<IEnumerable<CommentModel>> GetIssueComments(string issueKey, CancellationToken token = default);
        Task<IEnumerable<JiraLink>> GetIssueLinks(string issueKey, CancellationToken token = default);
        Task<IEnumerable<T>> GetIssues<T>(IEnumerable<string> keys, CancellationToken token = default) where T : IssueModel;
        IEnumerable<T> GetIssues<T>(string jql) where T : IssueModel;
        Task<IEnumerable<T>> GetLinks<T>(string issueKey, JiraLinkType linkType, CancellationToken token = default) where T : IssueModel;
        Task<IEnumerable<JiraUser>> GetUsers(string query, JiraUserStatus userStatus = JiraUserStatus.Active, CancellationToken token = default);
        Task<bool> ImportUsers(IEnumerable<NewUser> users, CancellationToken token = default);
        Task<bool> Link(string fromIssueKey, string toIssueKey, JiraLinkType linkType, CancellationToken token = default);
        Task<LogWork> SetLogWork(string issueKey, LogWork logWork, string updateUser = "", CancellationToken token = default);
        Task<bool> UpdateIssue<T>(T issueModel) where T : IssueModel;
        Task<InsightField> GetKeyOfValueInInsightField(string customFieldId, string optionValue);
        Task<IEnumerable<InsightField>> GetInsightFieldValues(string fieldId, int start, int limit);
        Task Delete(string issueKey, CancellationToken token = default);
    }
}