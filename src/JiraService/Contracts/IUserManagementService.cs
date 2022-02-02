using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JiraService.Consts;
using JiraService.Models;

namespace JiraService.Contracts
{
    public interface IUserManagementService
    {
        Task<IEnumerable<JiraUser>> GetUsers(string query, JiraUserStatus userStatus = JiraUserStatus.Active, CancellationToken token = default);
        Task<bool> ImportUsers(IEnumerable<NewUser> users, CancellationToken token = default);
    }
}
