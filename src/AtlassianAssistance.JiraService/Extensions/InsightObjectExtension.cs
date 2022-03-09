using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AtlassianAssistance.JiraService.Attributes;
using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.Extensions
{
    public static class InsightObjectExtension
    {
        private static MemberInfo GetMember<T>(
         Expression<Func<T, object>> selector)
        {
            var member = selector.Body as MemberExpression;
            if (member != null)
            {
                return member.Member;
            }
            return null;
        }

        private static T GetAttribute<T>(this MemberInfo meminfo) where T : Attribute
        {
            return meminfo.GetCustomAttributes(typeof(T)).FirstOrDefault() as T;
        }
        
        public static string GetCustomFieldId<T>(Expression<Func<T, object>> selector) where T : InsightObject
        {
            var member = GetMember(selector);
            var result = member.GetAttribute<CustomFieldAttribute>()?.FieldTypeId;
            if (result == null) throw new Exception($"This Property'{member.Name}' does not have 'CustomFieldAttribute' annotation with 'FieldTypeId'");
            return result;
        }

        public static int? GetFieldId<T>(Expression<Func<T, object>> selector) where T : InsightObject
        {
            var member = GetMember(selector);
            var result = member.GetAttribute<InsightAttributeFieldAttribute>()?.ObjectTypeAttributeId;
            if (result == null) throw new Exception($"This Property'{member.Name}' does not have 'CustomFieldAttribute' annotation with 'ObjectTypeAttributeId'");
            return result;
        }
    }
}
