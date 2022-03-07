using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.Models
{
    public abstract class InsightObject
    {
    }

    internal class InsightObjectAttribute
    {
        public int objectTypeAttributeId { get; set; }
        public InsightObjectAttributeValue[] objectAttributeValues { get; set; }
    }

    internal class InsightObjectAttributeValue
    {
        public string value { get; set; }

        public InsightObjectAttributeValue(string val)
        {
            value = val;
        }
    }
}
