using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.JiraInsightField
{
    public class TextJiField : JiraInsightFieldBase
    {
        public string Value { get; set; }
        protected internal override string[] GetInsightJiraValue => new[] { Value };

        public static explicit operator TextJiField(string val)
        {
            return new TextJiField() { Value = val };
        }
    }
}
