namespace AtlassianAssistance.JiraService.Models
{
    public class ValidateMessage
    {
        public ValidateMessage()
        {
            Valid = true;
        }
        public bool Valid { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return Message;
        }
    }
}