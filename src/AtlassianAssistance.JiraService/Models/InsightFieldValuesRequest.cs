using Atlassian.Jira;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlassianAssistance.JiraService.Services;

namespace AtlassianAssistance.JiraService.Models
{
    public class InsightFieldValuesRequest : CustomFieldValuesRequest<InsightField>
    {
        private readonly string fieldTypeId;
        private readonly bool includeAttributes;

        public InsightFieldValuesRequest(string fieldTypeId, bool includeAttributs = false)
        {
            this.fieldTypeId = fieldTypeId;
            this.includeAttributes = includeAttributs;
        }

        public override async Task<IEnumerable<InsightField>> GetValues(Jira jiraClient)
        {
            var fieldValues = new List<InsightField>();

            var objectTypeId = fieldTypeId;
            var page = 1;
            var resultsPerPage = 1000;

            do
            {
                var myObject = new { objectTypeId, page, resultsPerPage, includeAttributes };
                var result = await jiraClient.RestClient
                    .ExecuteRequestAsync(RestSharp.Method.POST, $"/rest/insight/1.0/object/navlist", myObject);

                var objects = JArray.Parse(result["objectEntries"].ToString());
                if (objects.Count == 0)
                    break;

                var values = objects.Select(x =>
                    new InsightField
                    {
                        Id = (int)x["id"],
                        Name = x["label"].ToString(),
                        Key = x["objectKey"].ToString(),
                        AttributesObject = x["attributes"] == null ? null : JArray.Parse(x["attributes"].ToString())
                    })
                    .ToList();

                if (includeAttributes)
                {
                    foreach (var value in values)
                    {
                        value.Attributes = value.AttributesObject?.Select(x =>
                            new InsightFieldAttribute
                            {
                                Id = (int)x["id"],
                                TypeId = (int)x["objectTypeAttribute"]["id"],
                                TypeName = x["objectTypeAttribute"]["name"].ToString(),
                                Values = JArray.Parse(x["objectAttributeValues"].ToString())
                                                            .Select(p => p["value"].ToObject<object>())
                                                            .ToArray(),
                                DisplayValues = JArray.Parse(x["objectAttributeValues"].ToString())
                                                            .Select(p => p["displayValue"].ToString())
                                                            .ToArray()
                            })
                            ?.ToList();
                    }
                }

                fieldValues.AddRange(values);
                page++;
            } while (true);

            return fieldValues;
        }
    }
}
