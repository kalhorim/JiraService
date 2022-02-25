using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.Models
{
    public class HistoryField
    {
        public string ModifiedBy { get; set; }
        public DateTime ModfiedAt { get; set; }
        public object FromValue { get; set; }
        public object ToValue { get; set; }
    }
}
