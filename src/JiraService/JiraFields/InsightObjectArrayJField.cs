using System;
using JiraService.Models;

namespace JiraService.JiraFields
{
    public class InsightObjectArrayJField : JiraCustomFieldBase
    {
        internal string[] Values { get; set; }
        public InsightField Value { get; set; }
        protected internal override string GetJiraValue => Value.Name;

        protected internal override string[] SetJiraValue { set => Value = ExtractValueFromInsightObject(value); }
        //TODO: Need Review
        private InsightField ExtractValueFromInsightObject(string[] values)
        {
            Values = values;
            //if (values.Length > 1) throw ThrowCastException(values);
            var value = values[0];
            var newValue = System.Text.RegularExpressions.Regex.Replace(value, @"\s\([^\)]+\)", "");
            return new InsightField { Name = newValue };
        }
    }
}
