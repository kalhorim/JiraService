using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.Contracts
{
    public interface IIssueService
    {
        ICommentService Comment { get; set; }

        Task SetTrackingTime(string issueKey, string originalEstimate, string remainingEstimate, CancellationToken token = default);
        Task<LogWork> SetLogWork(string issueKey, LogWork logWork, string updateUser = "", CancellationToken token = default);
        Task<IEnumerable<HistoryField>> GetHistory(string issueKey, string fieldName, CancellationToken token = default);
        Task ChangeStatus(string issueKey, string newStatus, string newResolution = null, CancellationToken token = default);
        Task<string> Create(IssueModel IssueModel, string parentIssueKey = null, CancellationToken token = default);
        Task<T> Get<T>(string issueKey, CancellationToken token = default) where T : IssueModel;
        Task<IEnumerable<T>> Get<T>(IEnumerable<string> keys, CancellationToken token = default) where T : IssueModel;
        IEnumerable<T> Query<T>(string jql) where T : IssueModel;
        IssueQueryResponse<T> Query<T>(IssueQueryRequest request) where T : IssueModel;
        Task<bool> Update<T>(T issueModel) where T : IssueModel;
        Task Delete(string issueKey, CancellationToken token = default);
    }
}
