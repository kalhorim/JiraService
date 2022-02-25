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
    public static class IssueModelExtensions
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
        public static string GetFieldId<T>(this T issueModel, Expression<Func<T, object>> selector) where T : IssueModel
        {
            return GetFieldId(selector);
        }
        public static string GetFieldId<T>(Expression<Func<T, object>> selector) where T : IssueModel
        {
            var member = GetMember(selector);
            var result = member.GetAttribute<CustomFieldAttribute>()?.FieldId;
            if (result == null) throw new Exception($"This Property'{member.Name}' does not have 'CustomFieldAttribute' annotation with 'FieldId'");
            return result;
        }

        public static string GetFieldName<T>(this T issueModel, Expression<Func<T, object>> selector) where T : IssueModel
        {
            return GetFieldName(selector);
        }
        public static string GetFieldName<T>(Expression<Func<T, object>> selector) where T : IssueModel
        {
            var member = GetMember(selector);
            var result = member.GetAttribute<CustomFieldAttribute>()?.Name;
            if (result == null) throw new Exception($"This Property'{member.Name}' does not have 'CustomFieldAttribute' annotation with 'Name'");
            return result;
        }
    }
}
