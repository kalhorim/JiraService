using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InsightAttributeFieldAttribute : Attribute
    {
        private readonly int objectTypeAttributeId;
        public InsightAttributeFieldAttribute(int objectTypeAttributeId)
        {
            this.objectTypeAttributeId = objectTypeAttributeId;
        }

        public int ObjectTypeAttributeId
        {
            get { return objectTypeAttributeId; }
        }
    }
}
