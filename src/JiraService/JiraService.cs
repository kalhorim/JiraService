using JiraService.Contracts;
using JiraService.Services;
using Microsoft.Extensions.Logging;

namespace JiraService
{
    internal class JiraService: IJiraService
    {
        public IIssueService Issue { get; set; }
        public IUserManagementService UserManagement { get; set; }
        public ILinkService LinkManagement { get; set; }
        public ISchemaService Schema { get; set; }
        public ICommentService Comment { get; set; }

        //TODO: Implement cache service
        internal JiraService(ILogger logger, Atlassian.Jira.Jira jira)
        {
            UserManagement = new UserManagementService(jira, logger);
            LinkManagement = new LinkService(jira, logger);
            Schema = new SchemaService(jira);
            Comment = new CommentService(jira);
            Issue = new IssueService(jira, logger);
        }
    }
}