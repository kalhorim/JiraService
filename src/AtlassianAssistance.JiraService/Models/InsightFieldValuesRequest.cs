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
        private readonly string fieldId;
        private readonly int start;
        private readonly int limit;
        private readonly bool includeAttribus;

        public InsightFieldValuesRequest(string fieldId, int start, int limit, bool includeAttribus = false)
        {
            this.fieldId = fieldId;
            this.start = start;
            this.limit = limit;
            this.includeAttribus = includeAttribus;
        }

        public override async Task<IEnumerable<InsightField>> GetValues(Jira jiraClient)
        {
            var myObject = new { start, limit };

            var result = await jiraClient.RestClient
                .ExecuteRequestAsync(RestSharp.Method.POST, $"/rest/insight/1.0/customfield/default/{RemovePrefix(fieldId)}/objects", myObject);

            var objects = Newtonsoft.Json.Linq.JArray.Parse(result["objects"].ToString());
            var values = objects.Select(Mapper.Map).ToList();

            if (includeAttribus)
            {
                await IncludeInsightFieldValueAttribus(jiraClient, values);
            }

            return values;
        }

        #region Private Methods
        private async Task IncludeInsightFieldValueAttribus(Jira jiraClient, List<InsightField> values)
        {
            foreach (var value in values)
            {
                var result = await jiraClient.RestClient
                                .ExecuteRequestAsync(RestSharp.Method.GET, $"/rest/insight/1.0/object/{value.Id}/attributes");

                var objects = JArray.Parse(result.ToString());
                value.Attributes = objects.Select(x =>
                    new InsightFieldAttribute
                    {
                        Id = (int)x["id"],
                        TypeId = (int)x["objectTypeAttribute"]["id"],
                        TypeName = x["objectTypeAttribute"]["name"].ToString(),
                        DisplayValues = JArray.Parse(x["objectAttributeValues"].ToString())
                                                    .Select(p => p["displayValue"].ToString())
                    })
                    .ToList();
            }
        }

        private string RemovePrefix(string fieldname)
        {
            return fieldname.Replace("customfield_", "");
        }
        #endregion
    }
}
