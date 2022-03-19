using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AtlassianAssistance.JiraService.JiraInsightField
{
    public class TextJiField : JiraInsightFieldBase
    {
        public string Value { get; set; }
        protected internal override string[] GetInsightJiraValue => new[] { Value };

        protected internal override void SetJiraValue(IEnumerable<object> value)
        {
            Value = value?.FirstOrDefault()?.ToString();
        }

        public static explicit operator TextJiField(string val)
        {
            return new TextJiField() { Value = val };
        }
    }
}
