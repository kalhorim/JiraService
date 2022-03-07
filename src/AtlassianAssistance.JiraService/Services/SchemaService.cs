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

        public async Task<string> CreateInsightObject(InsightObject insighObject)
        {
            var myObject = await GetInsightJsonObjectFromTypedObject(insighObject);
            var result = await _jiraClient.RestClient
                    .ExecuteRequestAsync(RestSharp.Method.POST, $"/rest/insight/1.0/object/create", myObject);
            var objectKey = result["objectKey"].ToString();
            return objectKey;
        }

        public async Task<string> UpdateInsightObject(InsightObject insighObject, string objectKeyValue)
        {
            var myObject = await GetInsightJsonObjectFromTypedObject(insighObject);
            var result = await _jiraClient.RestClient
                    .ExecuteRequestAsync(RestSharp.Method.PUT, $"/rest/insight/1.0/object/{objectKeyValue}", myObject);
            var objectKey = result["objectKey"].ToString();
            return objectKey;
        }
        #endregion

        #region Private Methods
        private async Task<dynamic> GetInsightJsonObjectFromTypedObject(InsightObject insighObject)
        {
            var objAttributes = new List<InsightObjectAttribute>();
            var objectType = insighObject.GetType();

            var attr = objectType.GetCustomAttributes(typeof(InsightObjectTypeAttribute), false).FirstOrDefault() as InsightObjectTypeAttribute;
            if (attr == null)
                throw new InvalidOperationException("Current InsighObject type don't have InsightAttributeField Attribute");
            var objectTypeId = attr.ObjectTypeId;

            var objectProperties = objectType
                    .GetProperties()
                    .Where(x => x.CustomAttributes.Any(c => c.AttributeType == typeof(InsightAttributeFieldAttribute)))
                    .ToList();

            foreach (var prop in objectProperties)
            {
                var value = prop.GetValue(insighObject);
                if (value == null)
                    continue;

                var attribute = await CreateObjectAttribute(prop, value);
                objAttributes.Add(attribute);
            }

            dynamic myObject = new { objectTypeId = objectTypeId, attributes = objAttributes.ToArray() };
            return myObject;
        }

        private async Task<InsightObjectAttribute> CreateObjectAttribute(System.Reflection.PropertyInfo prop, object value)
        {
            var attribute = new InsightObjectAttribute();
            attribute.objectTypeAttributeId = (prop.GetCustomAttributes(typeof(InsightAttributeFieldAttribute), false).First() as InsightAttributeFieldAttribute).ObjectTypeAttributeId;

            if (value is JiraCustomFieldBase customField)
            {
                var fieldTypeId = (prop.GetCustomAttributes(typeof(CustomFieldAttribute), false).First() as CustomFieldAttribute).FieldTypeId;
                if (value is InsightObjectArrayJField arrayField)
                {
                    attribute.objectAttributeValues = arrayField.Values.Select(x => new InsightObjectAttributeValue(GetKeyOfValueInInsightField(fieldTypeId, x).Result.Key)).ToArray();
                }
                else if (value is InsightJField x)
                {
                    attribute.objectAttributeValues = new InsightObjectAttributeValue[] { new InsightObjectAttributeValue((await GetKeyOfValueInInsightField(fieldTypeId, x.GetJiraValue)).Key) };
                }
                else if (value is JiraCustomFieldBase o)
                {
                    attribute.objectAttributeValues = new InsightObjectAttributeValue[] { new InsightObjectAttributeValue(o.GetJiraValue) };
                }
                return attribute;
            }

            if (value is string val)
            {
                attribute.objectAttributeValues = new InsightObjectAttributeValue[] { new InsightObjectAttributeValue(val) };
            }
            else if (value is bool vl)
            {
                attribute.objectAttributeValues = new InsightObjectAttributeValue[] { new InsightObjectAttributeValue(vl.ToString().ToLower()) };
            }
            else if (value is IEnumerable<string> values)
            {
                attribute.objectAttributeValues = values.Select(x => new InsightObjectAttributeValue(x)).ToArray();
            }
            else if (value is string[] arrValues)
            {
                attribute.objectAttributeValues = arrValues.Select(x => new InsightObjectAttributeValue(x)).ToArray();
            }

            return attribute;
        }
        #endregion
    }
}
