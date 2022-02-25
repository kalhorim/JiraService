using Microsoft.Extensions.Configuration;

namespace AtlassianAssistance.JiraService.Test.Service
{
    internal static class Consts
    {
        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }

        internal static SettingsModel GetSettingsModel()
        {
            var value = InitConfiguration();
            var parentSectionName = "Settings:";
            //TODO: use ServiceCollection
            return new SettingsModel
            {
                HOST = value[parentSectionName + "HOST"],
                USERNAME = value[parentSectionName + "USERNAME"],
                PASSWORD = value[parentSectionName + "PASSWORD"],
                IssueKey = value[parentSectionName + "IssueKey"]
            };
        }
    }
}