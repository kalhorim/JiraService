using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Models
{
    public class JiraLinkType
    {
        public enum ArrowType
        {
            Outward = 1,
            Inward = 0
        }
        public string Name { get; set; }
        public string OutwardDescription { get; set; }
        public string InwardDescription { get; set; }
        public ArrowType CurrentArrowType { get; }
        public string CurrentDescription => CurrentArrowType == ArrowType.Inward ? InwardDescription : OutwardDescription;
        public JiraLinkType()
        {

        }
    }
}

