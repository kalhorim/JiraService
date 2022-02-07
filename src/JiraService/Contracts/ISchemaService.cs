using System.Collections.Generic;
using System.Threading.Tasks;
using JiraService.Models;

namespace JiraService.Contracts
{
    public interface ISchemaService
    {
        Task<IEnumerable<CustomField>> GetCustomFields();
        Task<InsightField> GetKeyOfValueInInsightField(string customFieldId, string optionValue);
        Task<IEnumerable<T>> GetFieldValues<T>(CustomFieldValuesRequest<T> request);
    }
}
