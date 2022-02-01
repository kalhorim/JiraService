using System;
using System.Collections.Generic;
using JiraService.Models;

namespace JiraService.JiraFields
{
    public class InsightJField : JiraCustomFieldBase
    {
        public InsightField Value { get; set; }
        protected internal override string GetJiraValue => Value.Key;

        protected internal override string[] SetJiraValue { set => Value = ExtractValueFromInsightObject(value); }
        private InsightField ExtractValueFromInsightObject(string[] values)
        {
            if (values.Length != 1)
                throw CastException(values);
            var value = values[0];
            var regEx = @"\s\([^\)]+\)";
            var newValue = System.Text.RegularExpressions.Regex.Replace(value, regEx, "");
            var key = System.Text.RegularExpressions.Regex.Match(value, regEx).Value.Trim();
            key = key.Substring(1, key.Length - 2);
            int id;
            if (int.TryParse(key.Split("-")[1], out id))
                return new InsightField { Name = newValue, Key = key, Id = id };
            return new InsightField { Name = newValue, Key = key, Id = null };
        }

    }
}
