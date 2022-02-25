using AtlassianAssistance.JiraService.JiraFields;

namespace AtlassianAssistance.JiraService.Test.Model.CustomFields
{
    public class EpicLinkJField : JiraCustomFieldBase
    {
        private string _value;

        public string Value { get { return _value; } set => _value = value; }

        protected override string[] SetJiraValue { set => _value = string.Join("", value); }

        protected override string GetJiraValue => _value;
    }
}
