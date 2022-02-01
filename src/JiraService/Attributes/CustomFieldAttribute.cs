using System;
using System.Collections.Generic;
using System.Text;
using JiraService.JiraFields;
using JiraService.Models;

namespace JiraService.Attributes
{
    [AttributeUsage(AttributeTargets.Property,
                   AllowMultiple = false,
                   Inherited = true)]
    public sealed class CustomFieldAttribute : Attribute, IEquatable<Atlassian.Jira.CustomFieldValue>
    {
        public CustomFieldAttribute(string name, string fieldId = "", string defaultVal = null)
        {
            this.Name = name;
            this.FieldId = fieldId;
            this.DefaultVal = defaultVal;
        }

        public string Name { get; }

        public string FieldId { get; }

        public string DefaultVal { get; }

        public bool Equals(Atlassian.Jira.CustomFieldValue cfv)
        {
            return cfv != null 
                   && (
                       cfv.Id.Equals(FieldId, StringComparison.OrdinalIgnoreCase) 
                       || cfv.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)
                       );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FieldId, Name);
        }
        public override string ToString()
        {
            return $"FieldId:{FieldId}\tName:{Name}\tDefaultVal:{DefaultVal}";
        }
    }


}
