using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Atlassian.Jira;
using AtlassianAssistance.JiraService.Attributes;
using AtlassianAssistance.JiraService.Contracts;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using Newtonsoft.Json.Linq;
using RestSharp.Extensions;

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

        public async Task<string> CreateInsightObject<T>(T insighObject)
            where T : InsightObject
        {
            var myObject = MapToJsonInsightObject(insighObject);
            var result = await _jiraClient.RestClient
                    .ExecuteRequestAsync<T>(RestSharp.Method.POST, $"/rest/insight/1.0/object/create", myObject);
            
            insighObject.ObjectKey = result.ObjectKey;
            return result.ObjectKey.Value;
        }

        public async Task<string> UpdateInsightObject(InsightObject insighObject)
        {
            var objectKey = insighObject.ObjectKey.Value;
            if (string.IsNullOrEmpty(objectKey))
                throw new ArgumentNullException(nameof(insighObject.ObjectKey));

            var myObject = MapToJsonInsightObject(insighObject);
            var result = await _jiraClient.RestClient
                    .ExecuteRequestAsync(RestSharp.Method.PUT, $"/rest/insight/1.0/object/{objectKey}", myObject);
            objectKey = result["objectKey"].ToString();
            return objectKey;
        }
        #endregion

        #region Private Methods
        private object MapToJsonInsightObject(InsightObject insighObject)
        {
            var objAttributes = new List<InsightObjectAttribute>();

            var props = insighObject.GetInsightFields();
            foreach (var item in props)
            {
                objAttributes.Add(new InsightObjectAttribute()
                {
                    objectTypeAttributeId = item.Key,
                    objectAttributeValues = item.Value.GetInsightJiraValue.Select(InsightObjectAttributeValue.Parse).ToArray()
                });
            }

            object myObject = new { objectTypeId = insighObject.GetInsightObjectTypeId(), attributes = objAttributes.ToArray() };
            return myObject;
        }
        #endregion
    }
}
