using System.Collections;
using System.Collections.Generic;
using Atlassian.Jira;
using Microsoft.Extensions.Logging;
using Moq;
using AtlassianAssistance.JiraService;
using AtlassianAssistance.JiraService.Contracts;

namespace AtlassianAssistance.JiraService.Test.Service
{

    internal class JiraServiceProvider : IEnumerable<object[]>
    {
        private static readonly IJiraService _jiraWithCredentials;
        private static readonly SettingsModel setting;
        private readonly List<object[]> _data;

        //private static Jira _jiraWithOAuth;


        static JiraServiceProvider()
        {
            setting = Consts.GetSettingsModel();
            var mock = new Mock<ILogger>();
            var logger = mock.Object;
            _jiraWithCredentials =
                new JiraServiceCreator(logger).GetJiraService(setting.HOST, setting.USERNAME, setting.PASSWORD);


            /*_jiraWithOAuth = Jira.CreateOAuthRestClient(
                HOST,
                OAUTHCONSUMERKEY,
                OAUTHCONSUMERSECRET,
                OAUTHACCESSTOKEN,
                OAUTHTOKENSECRET);*/
        }

        public JiraServiceProvider()
        {
            _data = new List<object[]>
            {
                new object[] {_jiraWithCredentials}
                //new object[] { _jiraWithOAuth }
            };
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class JiraProvider : IEnumerable<object[]>
    {
        private static SettingsModel setting;

        private static Jira _jiraWithCredentials;
        //private static Jira _jiraWithOAuth;

        private readonly List<object[]> _data;

        public JiraProvider()
        {
            setting = Consts.GetSettingsModel();
            _jiraWithCredentials = Jira.CreateRestClient(setting.HOST, setting.USERNAME, setting.PASSWORD);

            /*_jiraWithOAuth = Jira.CreateOAuthRestClient(
               HOST,
               OAUTHCONSUMERKEY,
               OAUTHCONSUMERSECRET,
               OAUTHACCESSTOKEN,
               OAUTHTOKENSECRET);*/

            _data = new List<object[]>
            {
                new object[] {_jiraWithCredentials}
                //new object[] { _jiraWithOAuth }
            };
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}