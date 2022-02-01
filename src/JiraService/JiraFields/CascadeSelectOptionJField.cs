using System;

namespace JiraService.JiraFields
{
    public class CascadeSelectOptionJField : JiraCustomFieldBase
    {
        public string Value;

        protected internal override string[] SetJiraValue { set => Value = string.Join("", value); }

        protected internal override string GetJiraValue => prepareData();


        //TODO: Need a Review and a proposal
        private string prepareData()
        {
            var val = Value;
                bool isCascadeSelectOption = Value.Contains(" - ");
                if (isCascadeSelectOption)
                {
                    var splitedValue = Value.Split(new string[] { " - " }, StringSplitOptions.None);
                    var parent = splitedValue[0];
                    var child = splitedValue[1];
                    //issue.CustomFields.AddCascadingSelectField(attribute.Name, parent, child);
                }
            return val;
        }
    }
}
