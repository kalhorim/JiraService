using AtlassianAssistance.JiraService.Attributes;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.JiraInsightField;
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
        [InsightAttributeField(5484)]
        public TextJiField Name { get; set; }

        [InsightAttributeField(5487)]
        public TextJiField Category { get; set; }

        [InsightAttributeField(5488)]
        public BoolJiField CustomerFree { get; set; }

        [InsightAttributeField(5489)]
        public BoolJiField MustHaveRFC { get; set; }

        [CustomField("CI-Class", "13")]
        [InsightAttributeField(5490)]
        public InsightArrayJiField CiClasses { get; set; }
    }
}
