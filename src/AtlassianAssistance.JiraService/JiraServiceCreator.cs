﻿using Atlassian.Jira;
using Atlassian.Jira.OAuth;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using AtlassianAssistance.JiraService.Contracts;
using AtlassianAssistance.JiraService.Services.FieldSerializers;
using AtlassianAssistance.JiraService.Consts;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace AtlassianAssistance.JiraService
{
    public class JiraServiceCreator
    {
        private ILogger _logger { get; }

        private JiraRestClientSettings jsettings;

        public JiraServiceCreator(ILogger logger)
        {
            _logger = logger;
            jsettings = new JiraRestClientSettings();
        }
        public JiraServiceCreator()
        {
            _logger = new Logger<JiraServiceCreator>(new NullLoggerFactory());
            jsettings = new JiraRestClientSettings();
        }

        //TODO: Consider about open/close principle. 
        private JiraRestClientSettings AddCustomFieldSerializers()
        {
            jsettings.CustomFieldSerializers.Add("com.riadalabs.jira.plugins.insight:rlabs-customfield-default-object", new InsightCustomFieldValueSerializer());
            return jsettings;
        }
        public IJiraService GetJiraService(string url, string consumerKey, string consumerSecret, string oAuthAccessToken,
            string oAuthTokenSecret, JiraOAuthSignatureMethod oAuthSignatureMethod = JiraOAuthSignatureMethod.RsaSha1)
        {

            var _jiraClient = Atlassian.Jira.Jira.CreateOAuthRestClient(url,
                     consumerKey,
                     consumerSecret,
                     oAuthAccessToken, oAuthTokenSecret,
                     oAuthSignatureMethod,
                     AddCustomFieldSerializers()
                 );
            var service = new JiraService(_logger, _jiraClient);
            return service;
        }
        public IJiraService GetJiraService(string url, string username, string password)
        {
            var _jiraClient = Atlassian.Jira.Jira.CreateRestClient(url,
                     username, password,
                     AddCustomFieldSerializers()
                 );
            var service = new JiraService(_logger, _jiraClient);
            return service;
        }
    }
}
