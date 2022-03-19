using AtlassianAssistance.JiraService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlassianAssistance.JiraService.JiraInsightField
{
    public class InsightArrayJiField : JiraInsightFieldBase
    {
        public InsightField[] Values { get; set; }
        protected internal override string[] GetInsightJiraValue => Values?.Select(x => x.Key)?.ToArray();
        protected internal override void SetJiraValue(IEnumerable<object> value)
        {
            var values = new List<InsightField>();
            foreach (var val in value)
            {
                values.Add(new InsightField() { Key = val?.ToString() });
            }
            Values = values.ToArray();
        }
    }
}
