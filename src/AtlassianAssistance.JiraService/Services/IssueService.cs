using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using AtlassianAssistance.JiraService.Contracts;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using AtlassianAssistance.JiraService.Extensions;
using Microsoft.Extensions.Logging;

namespace AtlassianAssistance.JiraService.Services
{
    internal class IssueService: Contracts.IIssueService
    {
        public ICommentService Comment { get; set; }

        private readonly Jira _jiraClient;
        private readonly ILogger _logger;
        private readonly CommentService _commentService;
        private readonly SchemaService _schemaService;

        public IssueService(Jira jiraClient, ILogger logger)
        {
            _jiraClient = jiraClient;
            _logger = logger;
            _schemaService = new SchemaService(_jiraClient);
            _commentService = new CommentService(_jiraClient);
        }

        #region Public Methods
        #region Read
        public async Task<T> Get<T>(string issueKey, CancellationToken token = default) where T : IssueModel
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey, token);
            var issueModel = issue != null ? IssueMapper.Map<T>(issue) : null;
            return issueModel;
        }

        public IEnumerable<T> Query<T>(string jql) where T : IssueModel
        {
            var options = new IssueSearchOptions(jql)
            {
                FetchBasicFields = false,
                MaxIssuesPerRequest = 100,
            };

            var pageNumber = 1;
            _logger.LogInformation("Get issues starting ...");
            options.StartAt = 0;
            bool fetchedAny;
            do
            {
                var issues = _jiraClient.Issues.GetIssuesFromJqlAsync(options).Result;
                fetchedAny = issues.Any();


                pageNumber++;
                options.StartAt = ((pageNumber - 1) * options.MaxIssuesPerRequest.Value);

                foreach (var issue in issues)
                    yield return IssueMapper.Map<T>(issue);
            }
            while (fetchedAny);
        }

        public async Task<IEnumerable<HistoryField>> GetHistory(string issueKey, string fieldName, CancellationToken token = default)
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey, token);
            var logs = await issue.GetChangeLogsAsync(token);
            var result = new List<HistoryField>();
            foreach (var log in logs)
            {
                foreach (var item in log.Items)
                {
                    if (item.FieldName.ToLower() != fieldName.ToLower())
                        continue;
                    var history = Mapper.Map(item, log);
                    result.Add(history);
                }
            }
            return result;
        }
        public async Task<IEnumerable<T>> Get<T>(IEnumerable<string> keys, CancellationToken token = default) where T : IssueModel
        {
            var issues = await _jiraClient.Issues.GetIssuesAsync(keys, token);
            var issueModels = new List<T>();
            foreach (var issue in issues.Values)
            {
                var issueModel = IssueMapper.Map<T>(issue);
                issueModels.Add(issueModel);
            }
            return issueModels;
        }


        #endregion

        #region Create
        public async Task<string> Create(IssueModel subTaskIssueModel, string parentIssueKey = null, CancellationToken token = default)
        {
            return await createIssue(subTaskIssueModel, parentIssueKey, token);
        }

        public async Task SetTrackingTime(string issueKey, string originalEstimate, string remainingEstimate, CancellationToken token = default)
        {
            var editList = new List<object>();

            editList.Add(new { edit = new { originalEstimate = originalEstimate, remainingEstimate = remainingEstimate } });
            var myObject = new { update = new { timetracking = editList } };

            await _jiraClient.RestClient.ExecuteRequestAsync(RestSharp.Method.PUT, $"/rest/api/2/issue/{issueKey}", myObject, token);
        }
        #endregion

        #region Update
        public async Task<LogWork> SetLogWork(string issueKey, LogWork logWork, string updateUser = "", CancellationToken token = default)
        {
            var oldLogWorks = await _jiraClient.Issues.GetWorklogsAsync(issueKey, token);

            if (!string.IsNullOrEmpty(updateUser))
                foreach (var oldLogWork in oldLogWorks)
                {
                    if (oldLogWork.Author == updateUser)
                    {
                        await _jiraClient.Issues.DeleteWorklogAsync(issueKey, oldLogWork.Id);
                    }
                }

            var lw = new Atlassian.Jira.Worklog(logWork.TimeSpent, logWork.StartDate);
            lw.Comment = string.IsNullOrEmpty(logWork.Author) ? "" : $"Autor: {logWork.Author}";
            lw.Comment += $" {logWork.Comment}";

            var savedLogWork = await _jiraClient.Issues.AddWorklogAsync(issueKey, lw, token: token);
            var result = new LogWork(savedLogWork);
            return result;
        }

        public async Task ChangeStatus(string issueKey, string newStatus, string newResolution = default, CancellationToken token = default)
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey);
            await ChangeStatus(issue, newStatus, newResolution);
        }

        public async Task<bool> Update<T>(T issueModel)
           where T : IssueModel
        {
            try
            {
                Validate(issueModel);

                var oldIssue = await _jiraClient.Issues.GetIssueAsync(issueModel.Key);
                if (oldIssue == null)
                    throw new Exception($"issue {issueModel.Key} not found");

                issueModel.SetAttributeInCustomFields();
                oldIssue = IssueMapper.Map(oldIssue, issueModel);
                oldIssue.SaveChanges();

                await UpdateInsightObjectArrayFields(oldIssue, issueModel);

                var changes = ChangeDetector.GetChanges(issueModel).ToDictionary(x => x.PropertyName);

                if (changes.ContainsKey(nameof(IssueModel.Status)))
                {
                    var newValue = changes[nameof(IssueModel.Status)].NewValue.ToString();
                    if (oldIssue.Status.Name.ToLower() != newValue.ToLower())
                    {
                        await ChangeStatus(oldIssue, newValue, issueModel.Resolution);
                    }
                }

                if (changes.ContainsKey(nameof(IssueModel.RemainingEstimate)) || changes.ContainsKey(nameof(IssueModel.OriginalEstimate)))
                {
                    var orig = changes.ContainsKey(nameof(IssueModel.OriginalEstimate)) ? changes[nameof(IssueModel.OriginalEstimate)].NewValue : null;
                    var remain = changes.ContainsKey(nameof(IssueModel.RemainingEstimate)) ? changes[nameof(IssueModel.RemainingEstimate)].NewValue : null;

                    await SetTrackingTime(oldIssue.Key.Value, orig?.ToString(), remain?.ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in updating issue {issueModel.Key}");
                throw;
            }
        }
        #endregion

        #region Delete
        public async Task Delete(string issueKey, CancellationToken token = default)
        {
            await _jiraClient.Issues.DeleteIssueAsync(issueKey, token);
        }
        #endregion
        #endregion

        #region Private Methods


        private async Task<string> createIssue(IssueModel issueModel, string parentIssueKey = null, CancellationToken token = default)
        {
            Validate(issueModel);
            issueModel.SetAttributeInCustomFields();

            try
            {
                var issue = _jiraClient.CreateIssue(issueModel.ProjectKey, parentIssueKey);
                issue = IssueMapper.Map(issue, issueModel);
                issue.SaveChanges();
                await UpdateInsightObjectArrayFields(issue, issueModel);

                if (!string.IsNullOrEmpty(issueModel.OriginalEstimate) || !string.IsNullOrEmpty(issueModel.RemainingEstimate))
                    await SetTrackingTime(issue.Key.Value, issueModel.OriginalEstimate, issueModel.RemainingEstimate);

                if (issueModel.Status != null || issueModel.Resolution != null)
                    await ChangeStatus(issue, issueModel.Status, issueModel.Resolution);

                if (issueModel.Comments != null)
                    await issueModel.Comments.ToList().ForEachAsync(async comment => await _commentService.AddCommentAsync(issue.Key.Value, comment, token));

                if (issueModel.Attachments != null)
                    await _commentService.AddAttachmentsToIssue(issue, issueModel.Attachments);

                if (issueModel.LogWork != null)
                    await SetLogWork(issue.Key.Value, issueModel.LogWork);

                _logger.LogInformation($"Issue created {issue.Key.Value}");
                issueModel.Key = issue.Key.Value;
                return issue.Key.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in creating issue ");
                throw;
            }
        }

        private static void Validate(IssueModel issueModel)
        {
            if (!issueModel.IsValid(out var validates))
                throw new Exception(string.Join(Environment.NewLine, validates));
        }

        private async Task ChangeStatus(Issue issue, string newStatus, string newResolution)
        {
            if (string.IsNullOrEmpty(newStatus))
                return;

            if (!string.IsNullOrEmpty(newResolution))
            {
                issue.Resolution = newResolution;
            }

            await issue.WorkflowTransitionAsync(newStatus);
        }
        private async Task UpdateInsightObjectArrayFields(Issue issue, IssueModel issueModel)
        {
            var jiraAllCustomFields = (await _jiraClient.Fields.GetCustomFieldsAsync())
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x.First());
            await issueModel.GetCustomFields().Where(w => w.Value is InsightJField).Select(s => (InsightJField)s.Value).ToList().ForEachAsync(async customField =>
            {

                var attribute = customField.Attribute;
                var keyOfValues = new List<string>();

                var fieldTypeId = string.IsNullOrEmpty(attribute.FieldTypeId) ? customField.Value.FieldTypeId.ToString() : attribute.FieldTypeId;

                var keyOfValue = await _schemaService.GetKeyOfValueInInsightField(fieldTypeId, customField.Value.Name);
                keyOfValues.Add("{\"key\" : \"" + customField.Value.Key + "\"}");

                var customFieldIdentifier = jiraAllCustomFields[attribute.Name]?.Id;
                var keyValues = string.Join(",", keyOfValues);

                var isNewField = issue.CustomFields.Where(x => x.Name == attribute.Name).FirstOrDefault() == null;
                var myObjectString =
                        "{" +
                            "\"update\" : {" +
                                "\"" + customFieldIdentifier + "\" : [{\"UPDATE_METHOD\": [" + keyValues + "]}]" +
                            "}" +
                    "}";
                if (isNewField)
                {
                    myObjectString = myObjectString.Replace("UPDATE_METHOD", "set");
                }
                else
                {
                    myObjectString = myObjectString.Replace("UPDATE_METHOD", "add");
                }

                await _jiraClient.RestClient.ExecuteRequestAsync(RestSharp.Method.PUT, $"/rest/api/2/issue/{issue.Key}", myObjectString);

            });

        }
        #endregion
    }
}
