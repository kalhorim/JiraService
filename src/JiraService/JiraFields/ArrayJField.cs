using System;

namespace JiraService.JiraFields
{
    public class ArrayJField : JiraCustomFieldBase
    {
        protected internal override string GetJiraValue => throw new NotImplementedException();

        protected internal override string[] SetJiraValue { set => throw new NotImplementedException(); }
    }
}
