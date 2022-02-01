namespace JiraService.Test.Service
{

    internal class SettingsModel
    {
        public string HOST { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string OAUTHCONSUMERKEY { get; set; }
        public string OAUTHCONSUMERSECRET { get; set; }
        public string OAUTHACCESSTOKEN { get; set; }
        public string OAUTHTOKENSECRET { get; set; }
        public string IssueKey { get; set; }

        public string[] GetIssueKeys()
        {
            return IssueKey.Split(',');
        }
    }
}