using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;
using AtlassianAssistance.JiraService.Test.Model;
using AtlassianAssistance.JiraService.Test.Service;
using AtlassianAssistance.JiraService.Consts;
using AtlassianAssistance.JiraService.Contracts;
using AtlassianAssistance.JiraService.Extensions;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using Xunit;

namespace AtlassianAssistance.JiraService.Test.JiraServiceTest
{
    public class SchemaTest
    {
        [Theory]
        [ClassData(typeof(IssuesDataset))]
        public async void GetCustomFields_jiraService(IJiraService jiraService, string key)
        {
            var cfs = await jiraService.Schema.GetCustomFields();
            Assert.True(cfs.Any());
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_GetKeyOfValueInInsightField_Compare_jiraService(
            IJiraService jiraService)
        {
            var request = new InsightFieldValuesRequest(IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), 0, 100);
            var bservices = await jiraService.Schema.GetFieldValues(request);
            Assert.NotEmpty(bservices);
            var firstbs = bservices.FirstOrDefault();
            var insight =
                await jiraService.Schema.GetKeyOfValueInInsightField(
                    IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), firstbs.Name);
            Assert.Equal(firstbs, insight);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_CheckAfterAssign_jiraService(IJiraService jiraService)
        {
            var change = new ChangeIssue();
            var request = new InsightFieldValuesRequest(change.GetFieldId(f => f.BusinessService), 0, 100);
            change.BusinessService = new InsightJField
            {
                Value = (await jiraService.Schema.GetFieldValues(request)).LastOrDefault()
            };
            change.Customer = new InsightJField
            { Value = new InsightField { Id = 265, Key = "CRM-265", Name = "Test data" } };
            change.SetAttributeInCustomFields();
            await change.GetCustomFields().Where(w => w.Value is InsightJField).Select(s => (InsightJField)s.Value)
                .ToList().ForEachAsync(async customField =>
                {
                    var value = await jiraService.Schema.GetKeyOfValueInInsightField(customField.Attribute.FieldId,
                        customField.Value.Name);
                    Assert.Equal(value.Key, customField.Value.Key);
                });
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_jiraService(IJiraService jiraService)
        {
            var request = new InsightFieldValuesRequest(IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), 0, 100);
            var bservices = await jiraService.Schema.GetFieldValues(request);
            Assert.NotEmpty(bservices);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_InlcudeAttributes_jiraService(IJiraService jiraService)
        {
            var request = new InsightFieldValuesRequest(IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), 0, 100, true);
            var bservices = await jiraService.Schema.GetFieldValues(request);
            Assert.NotEmpty(bservices);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetFixVersions_jiraService(IJiraService jiraService)
        {
            var request = new VersionValuesRequest("PRD");
            var values = await jiraService.Schema.GetFieldValues(request);
            Assert.NotEmpty(values);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetDefaultLookupValues_jiraService(IJiraService jiraService)
        {
            var request = new DefaultLookupValuesRequest("SRV", "Change", IssueModelExtensions.GetFieldName<ChangeIssue>(f => f.ChangeType));
            var values = await jiraService.Schema.GetFieldValues(request);
            Assert.NotEmpty(values);
        }
    }
}