using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraService.Consts;
using JiraService.Contracts;
using JiraService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace JiraService.Services
{
    internal class UserManagementService: IUserManagementService
    {

        private readonly Jira _jiraClient;
        private readonly ILogger _logger;
        public UserManagementService(Jira jiraClient, ILogger logger)
        {
            _jiraClient = jiraClient;
            _logger = logger;
        }

        #region Public Methods
        public async Task<IEnumerable<Models.JiraUser>> GetUsers(string query, Consts.JiraUserStatus userStatus = Consts.JiraUserStatus.Active, CancellationToken token = default)
        {
            //TODO: must get all users by param
            var jiraUsers = await _jiraClient.Users.SearchUsersAsync(query, maxResults: 1000,
                userStatus: userStatus == Consts.JiraUserStatus.Active ? Atlassian.Jira.JiraUserStatus.Active :
                userStatus == Consts.JiraUserStatus.Inactive ? Atlassian.Jira.JiraUserStatus.Inactive : default, token: token);
            var activeJiraUsers = jiraUsers.Select(Mapper.Map);
            return activeJiraUsers;
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

        public async Task<IEnumerable<string>> GetGroups(int maxResults)
        {
            var result = await _jiraClient.RestClient
                .ExecuteRequestAsync(RestSharp.Method.GET, $"/rest/api/2/groups/picker?maxResults={maxResults}");

            var groups = JArray.Parse(result["groups"].ToString());
            var values = groups.Select(x => x["name"].ToString()).ToList();
            return values;
        }
        #endregion
    }
}
