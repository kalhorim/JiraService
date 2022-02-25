using Atlassian.Jira;
using AtlassianAssistance.JiraService.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianAssistance.JiraService.Models
{
    public class DefaultLookupValuesRequest : CustomFieldValuesRequest<KeyValuePair<string, object>>
    {
        private readonly string projectKey;
        private readonly string issueType;
        private readonly string fieldName;

        public DefaultLookupValuesRequest(string projectKey, string issueType, string fieldName)
        {
            this.projectKey = projectKey;
            this.issueType = issueType;
            this.fieldName = fieldName;
        }

        public override async Task<IEnumerable<KeyValuePair<string, object>>> GetValues(Jira jiraClient)
        {
            var allMetadatas = await GetFieldsCreateMetadataAsync(jiraClient, projectKey, issueType);
            if (allMetadatas == null || !allMetadatas.ContainsKey(fieldName))
                return Enumerable.Empty<KeyValuePair<string, object>>();

            var fieldMetaDatas = allMetadatas[fieldName];
            if (fieldMetaDatas == null)
                return Enumerable.Empty<KeyValuePair<string, object>>();

            var values = fieldMetaDatas.AllowedValues.Select(x => new
            {
                Id = (int)x["id"],
                Name = x["name"]?.ToString() ?? x["value"].ToString(),
                Children = x["children"]?.ToArray(),
                Key = x["id"].ToString(),
            });

            var result = new List<KeyValuePair<string, object>>();
            foreach (var value in values)
            {
                if (value.Children == null || value.Children.Length == 0)
                {
                    result.Add(new KeyValuePair<string, object>(value.Key, value.Name));
                    continue;
                }

                foreach (var valueChild in value.Children)
                {
                    result.Add(new KeyValuePair<string, object>(
                        valueChild["id"].ToString(),
                        value.Name + " - " + (valueChild["name"]?.ToString() ?? valueChild["value"].ToString())));
                }
            }

            return result;
        }

        #region Private Methods
        private async Task<IDictionary<string, IssueFieldEditMetadata>> GetFieldsCreateMetadataAsync(Jira jiraClient, string projectKey, string issuetypeName)
        {
            var dict = new Dictionary<string, IssueFieldEditMetadata>();
            var resource = String.Format("rest/api/2/issue/createmeta?projectKeys={0}&issuetypeNames={1}&expand=projects.issuetypes.fields", projectKey, issuetypeName);
            var serializer = Newtonsoft.Json.JsonSerializer.Create(jiraClient.RestClient.Settings.JsonSerializerSettings);
            var result = await jiraClient.RestClient.ExecuteRequestAsync(RestSharp.Method.GET, resource, null).ConfigureAwait(false);
            JObject fields = result["projects"]?.First?["issuetypes"]?.First?["fields"]?.Value<JObject>();

            if (fields == null)
                return dict;

            foreach (var prop in fields.Properties())
            {
                var fieldName = (prop.Value["name"] ?? prop.Name).ToString();
                dict.Add(fieldName, new Atlassian.Jira.IssueFieldEditMetadata(prop.Value.ToObject<Atlassian.Jira.Remote.RemoteIssueFieldMetadata>(serializer)));
            }

            return dict;
        }
        #endregion
    }
}
