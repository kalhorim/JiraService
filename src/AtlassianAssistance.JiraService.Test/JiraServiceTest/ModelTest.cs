using System.Linq;
using Atlassian.Jira;
using AtlassianAssistance.JiraService.Test.Model;
using AtlassianAssistance.JiraService.Test.Service;
using AtlassianAssistance.JiraService.Contracts;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using AtlassianAssistance.JiraService.Services;
using Xunit;

namespace AtlassianAssistance.JiraService.Test.JiraServiceTest
{

    public class ModelTest
    {
        [Fact]
        public void CheckValidation_CreateIssueModel()
        {
            var issue = new IssueModel();
            Assert.True(!issue.IsValid(out var validates));
            var validateMessages = validates as ValidateMessage[] ?? validates.ToArray();
            Assert.Contains(validateMessages, valid => valid.Message.Contains(nameof(issue.ProjectKey)));
            Assert.Contains(validateMessages, valid => valid.Message.Contains(nameof(issue.Type)));
            issue.ProjectKey = "test";
            issue.Type = "test";
            Assert.True(issue.IsValid(out validates));
            Assert.True(!validates.ToList().Any());
        }

        [Fact]
        public void GetSchema_CreateIssueModel()
        {
            var issue = new ChangeIssue();
            var schema = issue.GetCustomFields();
            Assert.True(schema.ContainsKey("Ticket Summary"));
        }

        [Fact]
        public void CheckTypeSafeAndDictionary_CreateIssueModel()
        {
            var changeTs = ChangeIssueInitializer.ChangeIssue.AddTypeSafeCustomFields();
            var change = ChangeIssueInitializer.ChangeIssue.AddCustomFields();
            var changeFields = change.GetCustomFields();
            changeTs.GetCustomFields().ToList().ForEach(f => { Assert.Equal(changeFields[f.Key], f.Value); });

            Assert.Equal(changeTs.BusinessService, change.BusinessService);
            Assert.Equal(changeTs.Customer, change.Customer);
            Assert.Equal(changeTs.PrimaryCustomer, change.PrimaryCustomer);
            Assert.Equal(changeTs.ChangeType, change.ChangeType);
        }

        [Fact]
        public void AssignCustomField_CheckCustomFieldAttribute_CreateIssueModel()
        {
            var change = new ChangeIssue();
            change.Customer = new InsightJField
                {Value = new InsightField {Id = 265, Key = "CRM-265", Name = "Amal Bank"}};
            Assert.Null(change.Customer.Attribute);
            change.SetAttributeInCustomFields();
            Assert.NotNull(change.Customer.Attribute);
        }

        [Theory]
        [ClassData(typeof(JiraProvider))]
        public void Map_IssueModel_JiraIssue(Jira jiraService)
        {
            var issueModel = new IssueModel();
            issueModel.ProjectKey = "SRV";
            var issue = jiraService.CreateIssue(issueModel.ProjectKey);
            issue = IssueMapper.Map(issue, issueModel);
            Assert.True(issue != null);
        }

        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void Map_JiraIssueToIsseModel(IJiraService jiraService, string key)
        {
            var changeIssue = await jiraService.Issue.Get<ChangeIssue>(key);
            Assert.NotNull(changeIssue.BusinessService.Attribute);
            Assert.NotNull(changeIssue.ChangeType);
            Assert.NotNull(changeIssue.Customer.Value);
        }
    }
}