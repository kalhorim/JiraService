﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using AtlassianAssistance.JiraService.Contracts;
using AtlassianAssistance.JiraService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AtlassianAssistance.JiraService.Services
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
            var links = await GetLinksForIssue(issueKey, token);
            return links;
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
            var links = await GetLinksForIssue(fromIssueKey, token);
            if (links.Any(l =>
                    (l.InwardIssueKey == toIssueKey || l.OutwardIssueKey == toIssueKey) &&
                    (l.InwardIssueKey == fromIssueKey || l.OutwardIssueKey == fromIssueKey) &&
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

        public async Task<bool> RemoveLink(string fromIssueKey, string toIssueKey, JiraLinkType linkType, CancellationToken token = default)
        {
            var links = await GetLinksForIssue(fromIssueKey, token);
            var linkId = links.FirstOrDefault(l =>
                    (l.InwardIssueKey == toIssueKey || l.OutwardIssueKey == toIssueKey) &&
                    (l.InwardIssueKey == fromIssueKey || l.OutwardIssueKey == fromIssueKey) &&
                    l.LinkType.Name == linkType.Name)
                ?.LinkId;

            if (linkId == null)
                return false;

            var resource = String.Format("/rest/api/2/issueLink/{0}", linkId);
            var result = await _jiraClient.RestClient.ExecuteRequestAsync(Method.DELETE, resource, null, token).ConfigureAwait(false);
            return true;

        }

        public async Task<IEnumerable<JiraLink>> GetLinksForIssue(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var serializerSettings = _jiraClient.RestClient.Settings.JsonSerializerSettings;
            var resource = String.Format("rest/api/2/issue/{0}?fields=issuelinks,created", issueKey);
            var issueLinksResult = await _jiraClient.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            var issueLinksJson = issueLinksResult["fields"]["issuelinks"];

            if (issueLinksJson == null)
            {
                throw new InvalidOperationException("There is no 'issueLinks' field on the issue data, make sure issue linking is turned on in JIRA.");
            }

            var issueLinks = issueLinksJson.Cast<JObject>();
            var filteredIssueLinks = issueLinks;

            var issuesToGet = filteredIssueLinks.Select(issueLink =>
            {
                var issueJson = issueLink["outwardIssue"] ?? issueLink["inwardIssue"];
                return issueJson["key"].Value<string>();
            }).ToList();


            return filteredIssueLinks.Select(issueLink =>
            {
                var linkType = JsonConvert.DeserializeObject<IssueLinkType>(issueLink["type"].ToString(), serializerSettings);
                var outwardIssue = issueLink["outwardIssue"];
                var inwardIssue = issueLink["inwardIssue"];
                var linkId = (int)issueLink["id"];
                var outwardIssueKey = outwardIssue != null ? (string)outwardIssue["key"] : null;
                var inwardIssueKey = inwardIssue != null ? (string)inwardIssue["key"] : null;
                return new JiraLink()
                {
                    LinkId = linkId,
                    LinkType = new JiraLinkType
                    {
                        Name = linkType.Name,
                        InwardDescription = linkType.Inward,
                        OutwardDescription = linkType.Outward
                    },
                    OutwardIssueKey = outwardIssueKey ?? issueKey,
                    InwardIssueKey = inwardIssueKey ?? issueKey
                };
            });
        }

        #endregion
    }
}
