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
            var request = new InsightFieldValuesRequest(IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService));
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
            var request = new InsightFieldValuesRequest(change.GetFieldId(f => f.BusinessService));
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
                    var value = await jiraService.Schema.GetKeyOfValueInInsightField(customField.Attribute.FieldTypeId,
                        customField.Value.Name);
                    Assert.Equal(value.Key, customField.Value.Key);
                });
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_jiraService(IJiraService jiraService)
        {
            var request = new InsightFieldValuesRequest(IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService));
            var bservices = await jiraService.Schema.GetFieldValues(request);
            Assert.NotEmpty(bservices);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_InlcudeAttributes_jiraService(IJiraService jiraService)
        {
            var request = new InsightFieldValuesRequest(IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), true);
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

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async Task CreateReleaseType_Test(IJiraService jiraService)
        {
            var request = new InsightFieldValuesRequest(InsightObjectExtension.GetCustomFieldId<ReleaseTypeInsightObject>(f => f.CiClasses), true);
            var allCiClasses = await jiraService.Schema.GetFieldValues(request);
            var ciClasses = allCiClasses.Where(x => new string[] { "Banco", "Card" }.Contains(x.Name)).ToArray();
            var obj = new ReleaseTypeInsightObject()
            {
                Name = new JiraInsightField.TextJiField { Value = "My Test" },
                MustHaveRFC = new JiraInsightField.BoolJiField { Value = false},
                CustomerFree = new JiraInsightField.BoolJiField { Value = true },
                Category = new JiraInsightField.TextJiField { Value = "Client" },
                CiClasses = new JiraInsightField.InsightArrayJiField() { Values = ciClasses }
            };
            string objectKey = await jiraService.Schema.CreateInsightObject(obj);
            Assert.NotNull(objectKey);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async Task UpdateReleaseType_Test(IJiraService jiraService)
        {
            var request = new InsightFieldValuesRequest(InsightObjectExtension.GetCustomFieldId<ReleaseTypeInsightObject>(f => f.CiClasses), true);
            var allCiClasses = await jiraService.Schema.GetFieldValues(request);
            var ciClasses = allCiClasses.Where(x => new string[] { "Banco" }.Contains(x.Name)).ToArray();

            var obj = new ReleaseTypeInsightObject()
            {
                ObjectKey = new JiraInsightField.TextJiField { Value = "CMDB-630991" },
                Name = new JiraInsightField.TextJiField { Value = "My Test" },
                MustHaveRFC = new JiraInsightField.BoolJiField { Value = false },
                CustomerFree = new JiraInsightField.BoolJiField { Value = true },
                Category = new JiraInsightField.TextJiField { Value = "Banco" },
                CiClasses = new JiraInsightField.InsightArrayJiField() { Values = ciClasses }
            };
            string objectKey = await jiraService.Schema.UpdateInsightObject(obj);
            Assert.NotNull(objectKey);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async Task GetInsightObject_Test(IJiraService jiraService)
        {
            var releaseType = await jiraService.Schema.GetInsightObject<ReleaseTypeInsightObject>("CMDB-442155");
            Assert.NotNull(releaseType);
        }
    }
}