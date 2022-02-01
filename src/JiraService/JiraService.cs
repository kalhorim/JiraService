using System;
using System.Collections.Generic;
using System.Text;
using JiraService.Extensions;
using System.Linq;
using JiraService.Attributes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using JiraService.Consts;
using Atlassian.Jira;
using JiraService.Contracts;
using JiraService.JiraFields;
using JiraService.Models;
using JiraService.Services;

namespace JiraService
{
    internal class JiraService : IJiraService
    {
        private readonly Atlassian.Jira.Jira _jiraClient;
        private readonly ILogger _logger;
        //TODO: Implement cache service
        internal JiraService(ILogger logger, Atlassian.Jira.Jira jira)
        {
            _logger = logger;
            _jiraClient = jira;
        }

        #region Public Methods

        #region Read
        public async Task<T> GetIssue<T>(string issueKey, CancellationToken token = default) where T : IssueModel
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey, token);
            var issueModel = issue != null ? IssueMapper.Map<T>(issue) : null;
            return issueModel;
        }

        public async Task<IEnumerable<T>> GetLinks<T>(string issueKey, JiraLinkType linkType, CancellationToken token = default) where T : IssueModel
        {
            var issue = await _jiraClient.Issues.GetIssueAsync(issueKey, token);
            var links = await issue.GetIssueLinksAsync(token);
            var filterLinks = links.Where(x => x.LinkType.Name == linkType.Name
                && x.LinkType.Outward == linkType.OutwardDescription);
            return filterLinks.Select(s => IssueMapper.Map<T>(s.OutwardIssue));
        }

        public async Task<IEnumerable<Models.JiraUser>> GetUsers(string query, Consts.JiraUserStatus userStatus = Consts.JiraUserStatus.Active, CancellationToken token = default)
        {
            //TODO: must get all users by param
            var jiraUsers = await _jiraClient.Users.SearchUsersAsync(query, maxResults: 1000,
                userStatus: userStatus == Consts.JiraUserStatus.Active ? Atlassian.Jira.JiraUserStatus.Active :
                userStatus == Consts.JiraUserStatus.Inactive ? Atlassian.Jira.JiraUserStatus.Inactive : default, token: token);
            var activeJiraUsers = jiraUsers.Select(Mapper.Map);
            return activeJiraUsers;
        }
        public async Task<IEnumerable<Models.CustomField>> GetCustomFields()
        {
            var customFields = (await _jiraClient.Fields.GetCustomFieldsAsync())
                .Select(Mapper.Map);
            return customFields;
        }

        public IEnumerable<T> GetIssues<T>(string jql) where T : IssueModel
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
        public async Task<IEnumerable<T>> GetIssues<T>(IEnumerable<string> keys, CancellationToken token = default) where T : IssueModel
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
        public async Task<IEnumerable<JiraLink>> GetIssueLinks(string issueKey, CancellationToken token = default)
        {
            var links = await _jiraClient.Links.GetLinksForIssueAsync(issueKey, token);
            return links.Select(Mapper.Map);
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

        public async Task<IEnumerable<InsightField>> GetInsightFieldValues(string fieldId, int start, int limit)
        {
            var myObject = new { start, limit };

            var result = await _jiraClient.RestClient
                .ExecuteRequestAsync(RestSharp.Method.POST, $"/rest/insight/1.0/customfield/default/{RemovePrefix(fieldId)}/objects", myObject);

            var objects = Newtonsoft.Json.Linq.JArray.Parse(result["objects"].ToString());
            var values = objects.Select(Mapper.Map).ToList();
            return values;
        }

        #endregion

        #region Create
        public async Task<string> CreateIssue(IssueModel subTaskIssueModel, string parentIssueKey = null, CancellationToken token = default)
        {
            return await createIssue(subTaskIssueModel, parentIssueKey, token);
        }

        public async Task AddTrackingTime(string issueKey, string originalEstimate, string remainingEstimate, CancellationToken token = default)
        {

            var editList = new List<object>();

            string original = "";
            string remaining = "";

            if (!string.IsNullOrEmpty(originalEstimate))
            {
                original = $"{originalEstimate}h";
            }

            if (!string.IsNullOrEmpty(remainingEstimate))
            {
                remaining = $"{remainingEstimate}h";
            }


            editList.Add(new { edit = new { originalEstimate = original, remainingEstimate = remaining } });
            var myObject = new { update = new { timetracking = editList } };

            await _jiraClient.RestClient.ExecuteRequestAsync(RestSharp.Method.PUT, $"/rest/api/2/issue/{issueKey}", myObject, token);

        }


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
        public async Task<bool> ImportUsers(IEnumerable<NewUser> users, CancellationToken token = default)
        {
            var errors = new Dictionary<string, string>();
            var index = 1;
            var count = users.Count();
            _logger.LogInformation("Import started.");
            foreach (var user in users)
            {
                _logger.LogInformation($"user {index++} : {count}");
                _logger.LogInformation($"Adding {user.Name} ...");
                var userObj = new { name = user.Name, displayName = user.DisplayName, emailAddress = user.EmailAddress, password = user.Password };
                try
                {
                    await _jiraClient.RestClient.ExecuteRequestAsync(RestSharp.Method.POST, "rest/api/2/user", userObj, token);
                }
                catch (Exception ex)
                {
                    errors[user.Name] = ex.Message;
                    _logger.LogError(ex, user.Name);
                }
            }
            _logger.LogInformation("Import started.");
            if (errors.Any()) throw new Exception(string.Join(Environment.NewLine, errors.Select(err => $"{err.Key}\t{err.Value}")));
            return true;
        }
        #endregion

        #region Update
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

        public async Task<bool> UpdateIssue<T>(T issueModel)
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

                    await AddTrackingTime(oldIssue.Key.Value, orig?.ToString(), remain?.ToString());
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
        #endregion Public Methods


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
                    await AddTrackingTime(issue.Key.Value, issueModel.OriginalEstimate, issueModel.RemainingEstimate);

                if (issueModel.Status != null || issueModel.Resolution != null)
                    await ChangeStatus(issue, issueModel.Status, issueModel.Resolution);

                if (issueModel.Comments != null)
                    await issueModel.Comments.ToList().ForEachAsync(async comment => await AddCommentAsync(issue.Key.Value, comment, token));

                if (issueModel.Attachments != null)
                    await AddAttachmentsToIssue(issue, issueModel.Attachments);

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

        public async Task<InsightField> GetKeyOfValueInInsightField(string customFieldId, string optionValue)
        {
            //TODO: Implement Caching
            return (await GetInsightFieldValues(customFieldId, 0, 1000)).SingleOrDefault(s => s.Name == optionValue);
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

                   var keyOfValue = await GetKeyOfValueInInsightField(attribute.FieldId, customField.Value.Name);
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

        private async Task AddAttachmentsToIssue(Issue issue, IEnumerable<AttachmentInfo> attachmentInfos)
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
        private string RemovePrefix(string fieldname)
        {
            return fieldname.Replace("customfield_", "");
        }
        #endregion
    }
}
