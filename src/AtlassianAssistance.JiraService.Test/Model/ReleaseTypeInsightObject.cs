using AtlassianAssistance.JiraService.Attributes;
using AtlassianAssistance.JiraService.JiraFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianAssistance.JiraService.Test.Model
{
    [InsightObjectType(884)]
    public class ReleaseTypeInsightObject : Models.InsightObject
    {
        [InsightAttributeField(5483)]
        public string ObjectKey { get; set; }
        [InsightAttributeField(5484)]
        public string Name { get; set; }

        [InsightAttributeField(5487)]
        public string Category { get; set; }

        [InsightAttributeField(5488)]
        public bool? CustomerFree { get; set; }

        [InsightAttributeField(5489)]
        public bool? MustHaveRFC { get; set; }

        [CustomField("CI-Class", "13")]
        [InsightAttributeField(5490)]
        public InsightObjectArrayJField CiClasses { get; set; }
    }
}
