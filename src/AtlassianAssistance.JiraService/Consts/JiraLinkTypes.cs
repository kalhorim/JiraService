using AtlassianAssistance.JiraService.Models;

namespace AtlassianAssistance.JiraService.Consts
{
    public static class JiraLinkTypes
    {
        public static JiraLinkType Blocks => new JiraLinkType
        {
            Name = "Blocks",
            InwardDescription = "is blocked by",
            OutwardDescription = "blocks"
        };
        public static JiraLinkType Cloners => new JiraLinkType
        {
            Name = "Cloners",
            InwardDescription = "is cloned by",
            OutwardDescription = "clones"
        };
        public static JiraLinkType Correlation => new JiraLinkType
        {
            Name = "Correlation",
            InwardDescription = "correlation",
            OutwardDescription = "correlation"
        };
        public static JiraLinkType Dependencies => new JiraLinkType
        {
            Name = "Dependencies",
            InwardDescription = "Successor",
            OutwardDescription = "Predecessor"
        };
        public static JiraLinkType Problem_Incident => new JiraLinkType
        {
            Name = "Problem/Incident",
            InwardDescription = "is caused by",
            OutwardDescription = "causes"
        };
        public static JiraLinkType Duplicate => new JiraLinkType
        {
            Name = "Duplicate",
            InwardDescription = "is duplicated by",
            OutwardDescription = "duplicates"
        };
        public static JiraLinkType Relates => new JiraLinkType
        {
            Name = "Relates",
            InwardDescription = "relates to",
            OutwardDescription = "relates to"
        };
        public static JiraLinkType Parent_Child => new JiraLinkType
        {
            Name = "Parent-Child",
            InwardDescription = "Child",
            OutwardDescription = "Parent"
        };
    }
}