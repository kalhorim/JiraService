namespace AtlassianAssistance.JiraService.JiraFields
{
    internal sealed class DefaultJField : JiraCustomFieldBase
    {
        public string Value { get; set; }
        protected internal override string[] SetJiraValue { set => Value = value.Length > 0 ? string.Join("", value) : null; }

        protected internal override string GetJiraValue => Value;

    }
}
