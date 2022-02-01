using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JiraService.Attributes;
using JiraService.JiraFields;
using JiraService.Contracts;
using JiraService.Extensions;

namespace JiraService.Models
{
    public abstract class IssueBase : Validate, IEquatable<IssueBase>
    {
        private void DeclareValidations()
        {
            AddValidatation(() =>
            {
                var projectKeyValid = new ValidateMessage();
                if (string.IsNullOrEmpty(ProjectKey))
                {
                    projectKeyValid.Message = $"{nameof(ProjectKey)} is required.";
                    projectKeyValid.Valid = false;
                }
                return projectKeyValid;
            });
            AddValidatation(() =>
            {
                var projectKeyValid = new ValidateMessage();
                if (string.IsNullOrEmpty(Type))
                {
                    projectKeyValid.Message = $"{nameof(Type)} is required.";
                    projectKeyValid.Valid = false;
                }
                return projectKeyValid;
            });

        }
        private IDictionary<string, IssueCustomFieldSchema> CustomFields { get; set; }
        public IssueBase()
        {
            DeclareValidations();
            CustomFields = new Dictionary<string, IssueCustomFieldSchema>();
        }
        public string Key { get; set; }
        public string ProjectKey { get; set; }
        public string Type { get; set; }
        public IDictionary<string, JiraCustomFieldBase> GetCustomFields()
        {
            return AsDictionary().ToDictionary(x => x.Value.customFieldAttribute.Name, x => x.Value.jiraCustomField);
        }
        public void RemoveCustomField(string key) { CustomFields.Remove(key); }
        public void SetCustomField(string key, JiraCustomFieldBase customField)
        {
            var propName = AsDictionary().SingleOrDefault(s => s.Value.customFieldAttribute.Name == key).Key ?? key;
            SetProperty(propName, new IssueCustomFieldSchema()
            {
                jiraCustomField = customField,
                JiraCustomFieldType = customField.GetType(),
                customFieldAttribute = new CustomFieldAttribute(key)
            });
        }
        internal void SetProperty(string key, IssueCustomFieldSchema customField)
        {
            GetType().GetProperty(key)?.SetValue(this, customField.jiraCustomField, null);
            CustomFields[key] = customField;
        }
        internal IDictionary<string, IssueCustomFieldSchema> AsDictionary()
        {
            var result = GetType()
              .GetProperties()
              .Where(x => x.CustomAttributes.Any(c => c.AttributeType == typeof(CustomFieldAttribute))
                       )
              .Where(x => typeof(JiraCustomFieldBase).IsAssignableFrom(x.PropertyType))
              .ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo =>
                {
                    var fieldattribute = propInfo.GetCustomAttributes(typeof(CustomFieldAttribute), false).First() as CustomFieldAttribute;
                    return new IssueCustomFieldSchema()
                    {
                        jiraCustomField = propInfo.GetValue(this) as JiraCustomFieldBase,
                        customFieldAttribute = fieldattribute,
                        JiraCustomFieldType = propInfo.PropertyType
                    };
                }
            );
            foreach (var item in result)
            {
                CustomFields[item.Key] = item.Value;
            }
            return CustomFields;
        }
        public void SetAttributeInCustomFields()
        {
            var customfields = AsDictionary();
            foreach (var schema in customfields)
            {
                if (schema.Value.jiraCustomField != null)
                    schema.Value.jiraCustomField.Attribute = schema.Value.customFieldAttribute;
            }
        }

        public bool Equals(IssueBase other)
        {
            return Key == other.Key;
        }
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    internal class IssueCustomFieldSchema
    {
        public CustomFieldAttribute customFieldAttribute { get; internal set; }
        public JiraCustomFieldBase jiraCustomField { get; set; }
        public Type JiraCustomFieldType { get; set; }
    }
}
