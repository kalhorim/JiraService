using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlassianAssistance.JiraService.JiraInsightField
{
    public class BoolJiField : JiraInsightFieldBase
    {
        public bool Value { get; set; }
        protected internal override string[] GetInsightJiraValue => new[] { Value.ToString().ToLower() };

        public static explicit operator BoolJiField(string val)
        {
            return new BoolJiField() { Value = string.IsNullOrEmpty(val) || val.ToLower() == "false" ? false : true };
        }

        protected internal override void SetJiraValue(IEnumerable<object> value)
        {
            Value = value?.FirstOrDefault()?.ToString()?.ToLower() == "true";
        }
    }
}
