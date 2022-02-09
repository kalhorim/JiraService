using JiraService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraService.Test.Model
{
    internal static class CustomLinkTypes
    {
        public static JiraLinkType AdditionalCustomer => new JiraLinkType
        {
            Name = "Additional Customer",
            InwardDescription = "Additional Customer issue",
            OutwardDescription = "Primary customer issue"
        };
    }
}
