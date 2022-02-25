using System;
using System.Collections.Generic;
using System.Linq;

namespace AtlassianAssistance.JiraService.Models
{
    public class Validate
    {
        private List<Func<ValidateMessage>> validations;
        protected void AddValidatation(Func<ValidateMessage> valid)
        {
            if (validations == null)
                validations = new List<Func<ValidateMessage>>();
            validations.Add(valid);
        }
        public bool IsValid(out IEnumerable<ValidateMessage> validates)
        {
            validates = ValidMessages()?.Where(w => !w.Valid);
            return !validates.Any();
        }
        public bool IsValid()
        {
            return IsValid(out _);
        }
        private IEnumerable<ValidateMessage> ValidMessages()
        {
            return validations?.Select(s => s.Invoke());
        }
    }
}