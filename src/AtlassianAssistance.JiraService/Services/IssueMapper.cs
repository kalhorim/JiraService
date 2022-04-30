﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlassianAssistance.JiraService.Attributes;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using AtlassianAssistance.JiraService.Extensions;

namespace AtlassianAssistance.JiraService.Services
{
    public static class IssueMapper
    {
        public static Atlassian.Jira.Issue Map(Atlassian.Jira.Issue issue, IssueModel issueModel)
        {
            issue = Map(issueModel, issue);
            issueModel.GetCustomFields().Where(w => w.Value != null && !(w.Value is DefaultJField)).ToList().ForEach(field =>
                {
                    field.Value.Assign(issue);
                });
            return issue;
        }
        public static T Map<T>(Atlassian.Jira.Issue issue) where T : IssueModel
        {
            var issueModel = (T)Activator.CreateInstance(typeof(T));

            issueModel = Map(issue, issueModel);
            var issueModelCustomfields = issueModel.AsDictionary();
            issue.CustomFields.ToList().ForEach(cf =>
            {
                var issueShema = issueModelCustomfields.FirstOrDefault(s => s.Value.customFieldAttribute.Equals(cf));
                if (issueShema.Key != null)
                {
                    if (issueShema.Value.jiraCustomField == null)
                        issueShema.Value.jiraCustomField = (JiraCustomFieldBase)Activator.CreateInstance(issueShema.Value.JiraCustomFieldType);
                    issueShema.Value.jiraCustomField.SetJiraValue = cf.Values;
                    issueShema.Value.jiraCustomField.Attribute = issueShema.Value.customFieldAttribute;
                    issueModel.SetProperty(issueShema.Key, issueShema.Value);
                }
                else
                {
                    issueModel.SetCustomField(cf.Name, new DefaultJField() { SetJiraValue = cf.Values, Attribute = new CustomFieldAttribute(cf.Name) });
                }
            });

            return issueModel;
        }

        #region private methods
        private static Atlassian.Jira.Issue Map(IssueModel fromissue, Atlassian.Jira.Issue toIssue)
        {
            toIssue.Type = fromissue.Type;
            toIssue.Reporter = fromissue.Reporter;
            toIssue.Summary = fromissue.Summary;
            toIssue.Description = fromissue.Description;
            toIssue.Assignee = fromissue.Assignee;
            if (fromissue.Labels != null)
            {
                toIssue.Labels.Clear();
                foreach (var label in fromissue.Labels)
                {
                    toIssue.Labels.Add(label);
                }
            }
            if (fromissue.FixVersions != null)
            {
                toIssue.FixVersions.Clear();
                foreach (var version in fromissue.FixVersions)
                {
                    toIssue.FixVersions.Add(version);
                }
            }
            if (fromissue.DueDate.HasValue)
            {
                toIssue.DueDate = fromissue.DueDate.Value;
            }
            return toIssue;
        }
        private static T Map<T>(Atlassian.Jira.Issue issue, T issueModel) where T : IssueModel
        {
            issueModel.Description = issue.Description;
            issueModel.OriginalEstimate = issue.TimeTrackingData?.OriginalEstimate;
            issueModel.ProjectKey = issue.Project;
            issueModel.RemainingEstimate = issue.TimeTrackingData?.RemainingEstimate;
            issueModel.Status = issue.Status.Name;
            issueModel.Resolution = issue.Resolution?.Name;
            issueModel.Reporter = issue.Reporter;
            issueModel.Summary = issue.Summary;
            issueModel.Type = issue.Type.Name;
            issueModel.Assignee = issue.Assignee;
            issueModel.Key = issue.Key.Value;
            issueModel.FixVersions = issue.FixVersions.Select(x => x.Name).ToArray();
            issueModel.Labels = issue.Labels.ToArray();

            return issueModel;
        }
        #endregion
    }
}
