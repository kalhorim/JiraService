using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlassianAssistance.JiraService.Consts;
using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.Contracts
{
    public interface IJiraService
    {
        IIssueService Issue { get; set; }
        IUserManagementService UserManagement { get; set; }
        ILinkService LinkManagement { get; set; }
        ISchemaService Schema { get; set; }
        ICommentService Comment { get; set; }
    }
}