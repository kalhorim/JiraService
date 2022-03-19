using AtlassianAssistance.JiraService.Attributes;
using AtlassianAssistance.JiraService.JiraInsightField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AtlassianAssistance.JiraService.Models
{
    public abstract class InsightObject
    {
        public TextJiField ObjectKey { get; set; }

        internal int GetInsightObjectTypeId()
        {
            var attr = this.GetType().GetCustomAttributes(typeof(InsightObjectTypeAttribute), false).FirstOrDefault() as InsightObjectTypeAttribute;
            if (attr == null)
                throw new InvalidOperationException("Current InsighObject type don't have InsightAttributeField Attribute");
           return attr.ObjectTypeId;
            
        }

        internal IDictionary<int, JiraInsightFieldBase> GetInsightFields()
        {
            var objectProperties = this.GetType()
                   .GetProperties()
                   .Where(x => x.PropertyType.BaseType == typeof(JiraInsightFieldBase))
                   .Where(x => x.CustomAttributes.Any(c => c.AttributeType == typeof(InsightAttributeFieldAttribute)))
                   .ToList();

            return objectProperties
                .Select(x => new 
                { 
                    Key = (x.GetCustomAttributes(typeof(InsightAttributeFieldAttribute), false).First() as InsightAttributeFieldAttribute).ObjectTypeAttributeId,
                    Value = x.GetValue(this) as JiraInsightFieldBase 
                })
                .ToDictionary(x => x.Key, x => x.Value);
        }

        internal IDictionary<int, PropertyInfo> GetInsightProperties()
        {
            var objectProperties = this.GetType()
                   .GetProperties()
                   .Where(x => x.PropertyType.BaseType == typeof(JiraInsightFieldBase))
                   .Where(x => x.CustomAttributes.Any(c => c.AttributeType == typeof(InsightAttributeFieldAttribute)))
                   .ToList();

            return objectProperties
                .Select(x => new
                {
                    Key = (x.GetCustomAttributes(typeof(InsightAttributeFieldAttribute), false).First() as InsightAttributeFieldAttribute).ObjectTypeAttributeId,
                    Value = x
                })
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }

    internal class InsightObjectAttribute
    {
        public int objectTypeAttributeId { get; set; }
        public InsightObjectAttributeValue[] objectAttributeValues { get; set; }
    }

    internal class InsightObjectAttributeValue
    {
        public string value { get; set; }

        public static InsightObjectAttributeValue Parse(string val)
        {
            return new InsightObjectAttributeValue() { value = val};
        }

    }
}
