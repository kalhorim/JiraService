using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraService.Models;
using JiraService.Contracts;
using Microsoft.Extensions.Logging;

namespace JiraService.Services
{
    internal class LinkService: ILinkService
    {

        private readonly Jira _jiraClient;
        private readonly ILogger _logger;
        public LinkService(Jira jiraClient, ILogger logger)
        {
            _jiraClient = jiraClient;
            _logger = logger;
        }

        #region Public Methods
        public async Task<IEnumerable<JiraLink>> GetIssueLinks(string issueKey, CancellationToken token = default)
        {
            var links = await _jiraClient.Links.GetLinksForIssueAsync(issueKey, token);
            return links.Select(Mapper.Map);
        }

        public async Task<IEnumerable<T>> GetLinks<T>(string issueKey, JiraLinkType linkType, CancellationToken token = default) where T : IssueModel
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey, token);
            var links = await issue.GetIssueLinksAsync(token);
            var filterLinks = links.Where(x => x.LinkType.Name == linkType.Name
                && x.LinkType.Outward == linkType.OutwardDescription);
            return filterLinks.Select(s => IssueMapper.Map<T>(s.OutwardIssue));
        }

        public async Task<bool> Link(string fromIssueKey, string toIssueKey, JiraLinkType linkType, CancellationToken token = default)
        {
            var links = await _jiraClient.Links.GetLinksForIssueAsync(fromIssueKey, token);
            if (links.Any(l =>
                    (l.InwardIssue.Key.Value == toIssueKey || l.OutwardIssue.Key.Value == toIssueKey) &&
                    (l.InwardIssue.Key.Value == fromIssueKey || l.OutwardIssue.Key.Value == fromIssueKey) &&
                    l.LinkType.Name == linkType.Name)
                   )
                return false;

            if (linkType.CurrentArrowType == JiraLinkType.ArrowType.Inward)
            {
                _logger.LogInformation($"Link from issue {fromIssueKey} To  issue {toIssueKey}");
                await _jiraClient.Links.CreateLinkAsync(fromIssueKey, toIssueKey, linkType.Name, linkType.CurrentDescription, token);
            }
            else
            {
                _logger.LogInformation($"Link from issue {toIssueKey} To  issue {fromIssueKey}");
                await _jiraClient.Links.CreateLinkAsync(toIssueKey, fromIssueKey, linkType.Name, linkType.CurrentDescription, token);
            }
            return true;

        }
        #endregion
    }
}
