using System;

namespace JiraService.JiraFields
{
    public class TextJField : JiraCustomFieldBase
    {
        private string _value;

        public string Value { get { return string.Format(Attribute.DefaultVal, _value); } set => _value = value; }

        protected internal override string[] SetJiraValue { set => _value = string.Join("", value); }

        protected internal override string GetJiraValue => _value;
    }
}
