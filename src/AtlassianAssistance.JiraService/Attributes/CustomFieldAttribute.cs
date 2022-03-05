using System;
using System.Collections.Generic;
using System.Text;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.Attributes
{
    [AttributeUsage(AttributeTargets.Property,
                   AllowMultiple = false,
                   Inherited = true)]
    public sealed class CustomFieldAttribute : Attribute, IEquatable<Atlassian.Jira.CustomFieldValue>
    {
        public CustomFieldAttribute(string name, string fieldTypeId = "", string defaultVal = null)
        {
            this.Name = name;
            this.FieldTypeId = fieldTypeId;
            this.DefaultVal = defaultVal;
        }

        public string Name { get; }

        public string FieldTypeId { get; }

        public string DefaultVal { get; }

        public bool Equals(Atlassian.Jira.CustomFieldValue cfv)
        {
            return cfv != null 
                   && (
                       cfv.Id.Equals(FieldTypeId, StringComparison.OrdinalIgnoreCase) 
                       || cfv.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)
                       );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + FieldTypeId.GetHashCode();
                hash = hash * 23 + Name.GetHashCode();
                return hash;
            }
        }
        public override string ToString()
        {
            return $"FieldId:{FieldTypeId}\tName:{Name}\tDefaultVal:{DefaultVal}";
        }
    }


}
