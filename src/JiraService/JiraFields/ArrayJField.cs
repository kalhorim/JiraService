using Atlassian.Jira;
using System;
using System.Linq;

namespace JiraService.JiraFields
{
    public class ArrayJField : JiraCustomFieldBase
    {
        public string[] Values { get; set; }

        protected internal override string GetJiraValue => Values == null ? null : string.Join(", ", Values);

        protected internal override string[] SetJiraValue { set => Values = value; }

        internal override void Assign(Issue issue)
        {
            var arrayField = issue.CustomFields.Where(x => x.Name == Attribute.Name).FirstOrDefault();
            if (arrayField == null)
            {
                issue.CustomFields.AddArray(Attribute.Name, Values);
            }
            else
            {
                arrayField.Values = Values;
            }
        }
    }
}
