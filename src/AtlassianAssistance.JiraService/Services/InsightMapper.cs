using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlassianAssistance.JiraService.Attributes;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using AtlassianAssistance.JiraService.Extensions;
using AtlassianAssistance.JiraService.JiraInsightField;

namespace AtlassianAssistance.JiraService.Services
{
    public static class InsightMapper
    {
        public static T Map<T>(this InsightField source) where T : InsightObject
        {
            var insightObject = (T)Activator.CreateInstance(typeof(T));

            insightObject.ObjectKey = new JiraInsightField.TextJiField() {  Value = source.Key };
            
            var props = insightObject.GetInsightProperties();

            source.Attributes.ToList().ForEach(at =>
            {
                if (props.ContainsKey(at.TypeId))
                {
                    var prop = props[at.TypeId];
                    var val = (Activator.CreateInstance(prop.PropertyType) as JiraInsightFieldBase);
                    val.SetJiraValue(at.Values);
                    prop.SetValue(insightObject, val);
                }
            });

            return insightObject;
        }
    }
}
