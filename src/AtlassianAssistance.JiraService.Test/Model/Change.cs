using System;
using AtlassianAssistance.JiraService.Attributes;
using AtlassianAssistance.JiraService.JiraFields;
using AtlassianAssistance.JiraService.Models;
using AtlassianAssistance.JiraService.Test.Model.CustomFields;

namespace AtlassianAssistance.JiraService.Test.Model
{

    public class ChangeIssue : IssueModel
    {
        #region ITicketIssue

        [CustomField("Ticket Code", defaultVal: "http://api/View/Tools/EntityDirect.aspx?etc=10292&no={0}")]
        public HyperLinkJField TicketCode { get; set; }

        [CustomField("Ticket Summary")] public TextJField TicketSummary { get; set; }

        [CustomField("Ticket Type")] public CascadeSelectOptionJField TicketType { get; set; }

        [CustomField("Ticket Business Service", "customfield_14204")]
        public InsightJField TicketBusinessService { get; set; }

        [CustomField("Ticket Environment Defected")]
        public CascadeSelectOptionJField TicketEnvironmentDefected { get; set; }

        [CustomField("Ticket Priority")] public CascadeSelectOptionJField TicketPriority { get; set; }

        [CustomField("Ticket Description")] public TextJField TicketDescription { get; set; }

        #endregion

        #region Custom Fields

        [CustomField("Created Time")] public DateTime CreatedTime { get; set; }

        [CustomField("BusinessPartner")] public CascadeSelectOptionJField BusinessPartner { get; set; }

        [CustomField("Change List")] public TextJField ChangeList { get; set; }

        [CustomField("Delivery Deadline")] public DateTimeJField DeliveryDeadline { get; set; }

        public DateTimeJField DeliveryDeadline1 { get; set; }

        [CustomField("Defected Release", "customfield_12619")]
        public InsightJField DefectedRelease { get; set; }

        [CustomField("Total Schedueled Duration")]
        public TextJField TotalScheduledDuration { get; set; }

        [CustomField("Environment Defected")] public CascadeSelectOptionJField EnvironmentDefected { get; set; }

        [CustomField("Requested Release", "customfield_12619")]
        public InsightJField RequestedRelease { get; set; }

        [CustomField("Basket", "customfield_12619")]
        public InsightJField Basket { get; set; }

        [CustomField("WI Number", defaultVal: "https://api/Production/_workitems?_a=edit&id={0}")]
        public HyperLinkJField WINumber { get; set; }

        [CustomField("Severity")] public CascadeSelectOptionJField Severity { get; set; }

        [CustomField("RFC Number", defaultVal: "http://api/View/Tools/EntityDirect.aspx?etc=10015&no={0}")]
        public HyperLinkJField RFCNumber { get; set; }

        [CustomField("Case Number", defaultVal: "http://api/View/Tools/EntityDirect.aspx?etc=112&no={0}")]
        public HyperLinkJField CaseNumber { get; set; }

        [CustomField("Primary Customer")] public CascadeSelectOptionJField PrimaryCustomer { get; set; }

        [CustomField("Customer", "customfield_10808")]
        public InsightJField Customer { get; set; }

        [CustomField("Change Type")] public TextJField ChangeType { get; set; }

        [CustomField("Business Service", "102")]
        public InsightJField BusinessService { get; set; }

        [CustomField("Enhancement")] public CascadeSelectOptionJField Enhancement { get; set; }

        [CustomField("Internal Letter Number",
            defaultVal: "http://api/View/Tools/EntityDirect.aspx?etc=4207&no={0}")]
        public HyperLinkJField LetterNumbers { get; set; }

        [CustomField("Ticket Number", defaultVal: "http://api/View/Tools/EntityDirect.aspx?etc=10292&no={0}")]
        public HyperLinkJField TicketNumbers { get; set; }

        [CustomField("Release CRM", defaultVal: "http://api/View/Tools/EntityDirect.aspx?etc=10023&no={0}")]
        public HyperLinkJField CrmReleases { get; set; }

        [CustomField("Epic Link")]
        public EpicLinkJField EpicLink { get; set; }
        #endregion
    }
}