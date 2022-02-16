using System;
using System.Collections.Generic;
using System.Text;
using JiraService.Attributes;

namespace JiraService.JiraFields
{
    public abstract class JiraCustomFieldBase
    {
        public CustomFieldAttribute Attribute { get; internal set; }
        public override string ToString()
        {
            return GetJiraValue;
        }
        public override bool Equals(object obj)
        {
            return ToString().Equals(obj.ToString());
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        protected internal abstract string GetJiraValue { get; }
        protected internal abstract string[] SetJiraValue { set; }
        protected Exception CastException(params string[] values)
        {
            return new InvalidCastException($"Cast exception\nAttribute: {Attribute}\nValue: {string.Join(",", values)}\nType: {GetType().FullName}");
        }

        internal virtual void Assign(Atlassian.Jira.Issue issue)
        {
            issue[Attribute.Name] = GetJiraValue;
        }
    }
}
