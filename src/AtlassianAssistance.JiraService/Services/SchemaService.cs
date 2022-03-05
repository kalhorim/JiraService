using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;
using AtlassianAssistance.JiraService.Contracts;
using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.Services
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

        public async Task<InsightField> GetKeyOfValueInInsightField(string customFieldTypeId, string optionValue)
        {
            //TODO: Implement Caching

            var request = new InsightFieldValuesRequest(customFieldTypeId);
            var values = await GetFieldValues(request);
            return values.SingleOrDefault(s => s.Name == optionValue);
        }

        public async Task<IEnumerable<T>> GetFieldValues<T>(CustomFieldValuesRequest<T> request)
        {
            return await request.GetValues(_jiraClient);
        }
        #endregion
    }
}
