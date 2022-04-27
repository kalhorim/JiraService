using System;
using System.Collections.Generic;
using System.Linq;

namespace AtlassianAssistance.JiraService.JiraFields
{
    public class HyperLinkArrayJField : JiraCustomFieldBase
    {
        public Dictionary<string, Uri> Values { get; set; }

        protected internal override string GetJiraValue => CombineData();

        protected internal override string[] SetJiraValue { set => ExtractLink(value); }

        //TODO: Use Regular expression, for more readable
        private void ExtractLink(string[] values)
        {
            Values = new Dictionary<string, Uri>();
            var vals = values[0]
                .Split(Environment.NewLine.ToCharArray())
                .Where(x => !string.IsNullOrEmpty(x));
            foreach (var value in vals)
            {
                var data = value.Split('|');
                if (data.Length != 2 || !data[0].StartsWith("[") || !data[1].EndsWith("]"))
                    throw CastException(value);
                var val = data[0].Substring(1, data[0].Length - 1);
                var link = new Uri(data[1].Substring(0, data[1].Length - 1));

                Values[val] = link;
            }
        }
        private string CombineData()
        {
            if (Values == null)
                return null;

            var links = Values.Select(x => $"[{x.Key}|{x.Value}]");

            return string.Join(Environment.NewLine, links);
        }
    }
}
