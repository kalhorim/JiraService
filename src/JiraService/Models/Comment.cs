using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Models
{
    public class CommentModel
    {
        public User User { get; set; }
        public string Body { get; set; }

    }
}
