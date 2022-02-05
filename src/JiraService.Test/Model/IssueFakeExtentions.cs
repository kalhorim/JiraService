using System;
using System.Collections.Generic;
using System.IO;
using JiraService.JiraFields;
using JiraService.Models;
using JiraService.Test.Model.CustomFields;

namespace JiraService.Test.Model
{

    public static class IssueFakeExtentions
    {
        private static readonly InsightJField newbs = new InsightJField()
            {Value = new InsightField {Id = 16653, Key = "CMDB-16653", Name = "991.58.0.4"}};

        public static T AddAttachment<T>(this T issue) where T : IssueModel
        {
            issue.Attachments = new List<AttachmentInfo>
            {
                BitmapAttachment(),
                TextFileAttachment()
            };
            return issue;
        }

        public static AttachmentInfo TextFileAttachment()
        {
            return new AttachmentInfo
            {
                Body = "attachment TextFile1 Test body",
                DataBytes = File.ReadAllBytes("Model/SampleAttachments/TextFile1.txt"),
                FileName = "TextFile1.txt",
                User = new User("Kalhori")
            };
        }

        public static AttachmentInfo BitmapAttachment()
        {
            return new AttachmentInfo
            {
                Body = "attachment Bitmap1 Test body",
                DataBytes = File.ReadAllBytes("Model/SampleAttachments/Bitmap1.bmp"),
                FileName = "Bitmap1.bmp",
                User = new User("Kalhori")
            };
        }

        public static T AddTypeSafeCustomFields<T>(this T issue) where T : ChangeIssue
        {
            issue.BusinessService = new InsightJField
                {Value = new InsightField {Id = 445640, Key = "CMDB-445640", Name = "telephonebank-4005.15.0.0"}};
            issue.Customer = new InsightJField
                {Value = new InsightField {Id = 265, Key = "CRM-265", Name = "Amal Bank"}};
            issue.PrimaryCustomer = new CascadeSelectOptionJField {Value = "Primary"};
            issue.ChangeType = new TextJField {Value = "Standard"};
            issue.EpicLink = new EpicLinkJField { Value = "SRV-12325" };
            return issue;
        }

        public static T AddCustomFields<T>(this T issue) where T : ChangeIssue
        {
            issue.SetCustomField("Business Service",
                new InsightJField
                    {Value = new InsightField {Id = 445640, Key = "CMDB-445640", Name = "telephonebank-4005.15.0.0"}});
            issue.SetCustomField("Customer",
                new InsightJField {Value = new InsightField {Id = 265, Key = "CRM-265", Name = "Amal Bank"}});
            issue.SetCustomField("Primary Customer", new CascadeSelectOptionJField {Value = "Primary"});
            issue.SetCustomField("Change Type", new TextJField {Value = "Standard"});
            issue.SetCustomField("Epic Link", new EpicLinkJField { Value = "SRV-12325" });
            return issue;
        }

        public static T UpdateCustomFields<T>(this T issue) where T : ChangeIssue
        {
            issue.BusinessService = newbs;
            return issue;
        }

        public static bool IsUpdated<T>(this T issue) where T : ChangeIssue
        {
            return issue.BusinessService.Equals(newbs);
        }

        public static T AddStatus<T>(this T issue, string status = "PLANNING") where T : IssueModel
        {
            issue.Status = status;
            issue.Resolution = "";
            return issue;
        }

        public static T AddComment<T>(this T issue) where T : IssueModel
        {
            issue.Comments = new List<CommentModel>
            {
                new AttachmentInfo()
                {
                    Body = "TEST Comment",
                    User = new User("Kalhori")
                }
            };
            return issue;
        }

        //TODO: this method need a review
        public static T SetLogWork<T>(this T issue) where T : IssueModel
        {
            issue.LogWork = new LogWork(string.Empty, DateTime.Now) {Author = "Kalhori", Comment = "SetLogWork Test"};
            return issue;
        }

        //TODO: this method need a review
        public static T AddTrackingTime<T>(this T issue) where T : IssueModel
        {
            issue.OriginalEstimate = "";
            issue.RemainingEstimate = "";
            return issue;
        }
    }
}