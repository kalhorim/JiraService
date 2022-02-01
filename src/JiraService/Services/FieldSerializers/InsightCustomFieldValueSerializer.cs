using Atlassian.Jira;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Services.FieldSerializers
{
    internal class InsightCustomFieldValueSerializer : ICustomFieldValueSerializer
    {
        public string[] FromJson(JToken json)
        {
            return new string[] { json[0].ToString() };
        }

        public JToken ToJson(string[] values)
        {
            var token = JToken.Parse("[{\"key\" : \"" + values[0] + "\"}]");
            return token;
        }
    }
}