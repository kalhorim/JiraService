using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.Contracts
{
    public interface ILinkService
    {
        Task<IEnumerable<JiraLink>> GetIssueLinks(string issueKey, CancellationToken token = default);
        Task<IEnumerable<T>> GetLinks<T>(string issueKey, JiraLinkType linkType, CancellationToken token = default) where T : IssueModel;
        Task<bool> Link(string fromIssueKey, string toIssueKey, JiraLinkType linkType, CancellationToken token = default);
    }
}
