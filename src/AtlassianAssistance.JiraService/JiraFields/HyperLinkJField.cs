using System;

namespace AtlassianAssistance.JiraService.JiraFields
{
    public class HyperLinkJField : JiraCustomFieldBase
    {
        public Uri Link { get; set; }

        public string Value { get; set; }

        protected internal override string GetJiraValue => CombineData();

        protected internal override string[] SetJiraValue { set => ExtractLink(value); }

        //TODO: Use Regular expression, for more readable
        private void ExtractLink(string[] value)
        {
            var data = value[0].Split('|');
            if (value.Length > 1 || data.Length != 2 || !data[0].StartsWith("[") || !data[1].EndsWith("]"))
            {
                Value = null;
                Link = null;
                return;
            }
            Value = data[0].Substring(1, data[0].Length-1);
            Link = new Uri(data[1].Substring(0, data[1].Length - 1));
        }
        private string CombineData()
        {
            return $"[{Value}|{Link}]";
        }
    }
}
