using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Models
{
    public class User
    {
        public User(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
            Username = username;
        }

        public string Username { get; internal set; }
    }
    public class NewUser : User
    {
        public NewUser(string username) : base(username) { }
        public string Name { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
    }
}
