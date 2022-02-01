using Atlassian.Jira.OAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Models
{
    public class BasicAuthenticationSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
    }

    public class OAuth2Settings
    {
        internal OAuthAccessTokenSettings OAuthAccessTokenSettings { get; set; }
        internal string OAuthTokenSecret { get; set; }
        internal string OAuthToken { get; set; }
        internal string ConsumerSecret { get; set; }
        internal string AccessToken { get; set; }
        internal string Username { get; set; }

        public string Url { get; set; }
        public string ConsumerKey { get; set; }
        public string PrivateKey { get; set; }
        public string AuthorizationCallback { get; set; }
    }
}
