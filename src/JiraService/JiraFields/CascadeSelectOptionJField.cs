using System;
using System.Linq;

namespace JiraService.JiraFields
{
    public class CascadeSelectOptionJField : JiraCustomFieldBase
    {
        public string Value;

        protected internal override string[] SetJiraValue { set => Value = string.Join("", value); }

        protected internal override string GetJiraValue => Value;

        internal override void Assign(Atlassian.Jira.Issue issue)
        {
            bool isParentChildCascadeSelectOption = Value.Contains(" - ");
            if (isParentChildCascadeSelectOption)
            {
                var splitedValue = Value.Split(new string[] { " - " }, StringSplitOptions.None);
                var parent = splitedValue[0];
                var child = splitedValue[1];
                var customFieldId = issue.CustomFields.SingleOrDefault(x => x.Name == Attribute.Name)?.Id;
                if (!string.IsNullOrEmpty(customFieldId))
                {
                    issue.CustomFields[Attribute.Name].Values = new string[] { parent, child};
                }
                else
                {
                    issue.CustomFields.AddCascadingSelectField(Attribute.Name, parent, child);
                }
            }
            else
            {
                var customFieldId = issue.CustomFields.SingleOrDefault(x => x.Name == Attribute.Name)?.Id;
                if (!string.IsNullOrEmpty(customFieldId))
                {
                    issue[Attribute.Name] = Value;
                }
                else
                {
                    issue.CustomFields.AddCascadingSelectField(Attribute.Name, Value);
                }
            }
        }
    }
}
