using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;
using JiraService.Test.Model;
using JiraService.Test.Service;
using JiraService.Consts;
using JiraService.Contracts;
using JiraService.Extensions;
using JiraService.JiraFields;
using JiraService.Models;
using Xunit;

namespace JiraService.Test.JiraServiceTest
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
            var bservices =
                await jiraService.Schema.GetInsightFieldValues(
                    IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), 0, 100);
            Assert.NotEmpty(bservices);
            var firstbs = bservices.FirstOrDefault();
            var insight =
                await jiraService.Schema.GetKeyOfValueInInsightField(
                    IssueModelExtensions.GetFieldId<ChangeIssue>(f => f.BusinessService), firstbs.Name);
            Assert.Equal(firstbs, insight);
        }

        [Theory]
        [ClassData(typeof(JiraServiceProvider))]
        public async void GetInsightFieldValues_jiraService(IJiraService jiraService)
        {
            var change = new ChangeIssue();
            change.BusinessService = new InsightJField
            {
                Value = (await jiraService.Schema.GetInsightFieldValues(change.GetFieldId(f => f.BusinessService), 0, 100))
                    .LastOrDefault()
            };
            change.Customer = new InsightJField
                {Value = new InsightField {Id = 265, Key = "CRM-265", Name = "Test data"}};
            change.SetAttributeInCustomFields();
            await change.GetCustomFields().Where(w => w.Value is InsightJField).Select(s => (InsightJField) s.Value)
                .ToList().ForEachAsync(async customField =>
                {
                    var value = await jiraService.Schema.GetKeyOfValueInInsightField(customField.Attribute.FieldId,
                        customField.Value.Name);
                    Assert.Equal(value.Key, customField.Value.Key);
                });
        }
    }
}