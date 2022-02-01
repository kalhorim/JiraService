using JiraService.Models;

namespace JiraService.Services
{
    internal static class Mapper
    {
        internal static JiraUser Map(Atlassian.Jira.JiraUser user)
        {
            return new JiraUser
            {
                Active = user.IsActive,
                DisplayName = user.DisplayName,
                EmailAddress = user.Email,
                Key = user.Key,
                Name = user.DisplayName
            };
        }
        internal static CustomField Map(Atlassian.Jira.CustomField customField)
        {
            return new CustomField
            {
                CustomIdentifier = customField.CustomIdentifier,
                CustomType = customField.CustomType,
                Id = customField.Id,
                Name = customField.Name,
            };
        }
        internal static InsightField Map(Newtonsoft.Json.Linq.JToken jToken)
        {
            return new InsightField
            {
                Id = (int)jToken["id"],
                Name = jToken["label"].ToString(),
                Key = jToken["objectKey"].ToString(),
            };
        }
        internal static HistoryField Map(Atlassian.Jira.IssueChangeLogItem item, Atlassian.Jira.IssueChangeLog log)
        {
            return new HistoryField
            {
                FromValue = item.FromValue,
                ToValue = item.ToValue,
                ModfiedAt = log.CreatedDate,
                ModifiedBy = log.Author.Username
            };
        }
        internal static AttachmentInfo Map(Atlassian.Jira.Attachment attachment)
        {
            return new AttachmentInfo
            {
                FileName = attachment.FileName,
                DataBytes = attachment.DownloadData(),
                User = new User(attachment.Author),
            };
        }
        internal static JiraLink Map(Atlassian.Jira.IssueLink issueLink)
        {
            return new JiraLink
            {
                InwardIssueKey = issueLink.InwardIssue.Key.Value,
                OutwardIssueKey = issueLink.OutwardIssue.Key.Value,
                LinkType = new JiraLinkType
                {
                    Name = issueLink.LinkType.Name,
                    InwardDescription = issueLink.LinkType.Inward,
                    OutwardDescription = issueLink.LinkType.Outward
                }
            };
        }
        internal static CommentModel Map(Atlassian.Jira.Comment comment)
        {
            return new CommentModel
            {
                User = new User(comment.Author),
                Body = comment.Body,
            };
        }
        internal static Atlassian.Jira.Comment Map(CommentModel comment)
        {
            return new Atlassian.Jira.Comment
            {
                Author = comment.User.Username,
                Body = comment.Body,
            };
        }

    }
}
