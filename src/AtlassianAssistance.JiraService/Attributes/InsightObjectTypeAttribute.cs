using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.Attributes
{
    public sealed class InsightObjectTypeAttribute : Attribute
    {
        private readonly int objectTypeId;
        public InsightObjectTypeAttribute(int objectTypeId)
        {
            this.objectTypeId = objectTypeId;
        }

        public int ObjectTypeId
        {
            get { return objectTypeId; }
        }
    }
}
