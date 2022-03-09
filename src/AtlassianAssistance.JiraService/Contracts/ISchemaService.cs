using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.Contracts
{
    public interface ISchemaService
    {
        Task<IEnumerable<CustomField>> GetCustomFields();
        Task<InsightField> GetKeyOfValueInInsightField(string customFieldId, string optionValue);
        Task<IEnumerable<T>> GetFieldValues<T>(CustomFieldValuesRequest<T> request);
        Task<string> CreateInsightObject<T>(T insighObject) where T : InsightObject;
        Task<string> UpdateInsightObject(InsightObject insighObject);
    }
}
