using System;

namespace JiraService.JiraFields
{
    public class ArrayJField : JiraCustomFieldBase
    {
        public string[] Values { get; set; }

        protected internal override string GetJiraValue => Values == null ? null : string.Join(", ", Values);

        protected internal override string[] SetJiraValue { set => Values = value; }
    }
}
