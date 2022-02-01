using System;
using System.Collections.Generic;
using System.Text;
using JiraService.Extensions;

namespace JiraService.Models
{
    public class LogWork
    {
        public string TimeSpent { get; }
        public DateTime StartDate { get; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public string Id { get; }
        public long TimeSpentInSeconds { get; }
        public DateTime? CreateDate { get; }
        public DateTime? UpdateDate { get; }

        public LogWork(string timeSpent, DateTime startDate)
        {
            TimeSpent = GetTimeSpent(timeSpent);
            StartDate = startDate;
        }

        internal LogWork(Atlassian.Jira.Worklog worklog)
        {
            TimeSpent = worklog.TimeSpent;
            if(worklog.StartDate.HasValue)
                StartDate = worklog.StartDate.Value;

            Author = worklog.Author;
            Comment = worklog.Comment;
            Id = worklog.Id;
            TimeSpentInSeconds = worklog.TimeSpentInSeconds;
            CreateDate = worklog.CreateDate;
            UpdateDate = worklog.UpdateDate;
        }

        private string GetTimeSpent(string time)
        {
            if (string.IsNullOrEmpty(time))
                return null;

            if (time.Contains(":"))
            {
                var timeParts = time.Split(':');
                if (timeParts.Length == 3)
                {
                    var hourPart = decimal.Parse(timeParts[0]);
                    var minutePart = decimal.Parse(timeParts[1]);
                    
                    if (hourPart > 0 || minutePart > 0)
                        return $"{hourPart}h {minutePart}m";
                }
            }
            else
            {
                if (decimal.TryParse(time, out decimal hour) && hour > 0)
                    return $"{hour}h";
            }

            return null;
        }
    }
}
