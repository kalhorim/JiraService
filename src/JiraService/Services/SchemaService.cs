using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraService.Models;
using JiraService.Contracts;

namespace JiraService.Services
{
    internal class SchemaService: ISchemaService
    {
        private readonly Jira _jiraClient;
        public SchemaService(Jira jiraClient)
        {
            _jiraClient = jiraClient;
        }

        #region Public Methods
        public async Task<IEnumerable<Models.CustomField>> GetCustomFields()
        {
            var customFields = (await _jiraClient.Fields.GetCustomFieldsAsync())
                .Select(Mapper.Map);
            return customFields;
        }

        public async Task<InsightField> GetKeyOfValueInInsightField(string customFieldId, string optionValue)
        {
            //TODO: Implement Caching
            return (await GetInsightFieldValues(customFieldId, 0, 1000)).SingleOrDefault(s => s.Name == optionValue);
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

        #region Private Methods
        private string RemovePrefix(string fieldname)
        {
            return fieldname.Replace("customfield_", "");
        }
        #endregion
    }
}
