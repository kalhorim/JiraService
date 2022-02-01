using System;

namespace JiraService.JiraFields
{
    public class DateTimeJField : JiraCustomFieldBase
    {
        public DateTime Value { get; set; }
        protected internal override string GetJiraValue => Map(Value);

        protected internal override string[] SetJiraValue { set => Value = Map(value); }
        private string Map(DateTime dt)
        {
            var localdt = new DateTime(dt.Ticks, DateTimeKind.Local);
            var dateTimeStr = localdt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz");
            dateTimeStr = dateTimeStr.Remove(dateTimeStr.LastIndexOf(":"), 1);
            return dateTimeStr;
        }
        private DateTime Map(string[] jiraVal)
        {
            if (DateTime.TryParse(jiraVal[0], out var result))
                return result;
            throw CastException(jiraVal);
        }
    }
}
