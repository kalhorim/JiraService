using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AtlassianAssistance.JiraService.Services
{
    public class ChangeLog
    {
        public string ClassName { get; set; }
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }

    internal static class ChangeDetector
    {
        internal static List<ChangeLog> GetChanges<T>(T changedObject)
        {
            var objType = typeof(T);
            var rawObj = Activator.CreateInstance(objType);

            List<ChangeLog> logs = new List<ChangeLog>();

            var properties = objType.GetProperties();

            var className = objType.Name;

            foreach (var property in properties)
            {
                var oldValue = property.GetValue(rawObj);
                var newValue = property.GetValue(changedObject);
                if (oldValue != newValue)
                {
                    logs.Add(new ChangeLog()
                    {
                        ClassName = className,
                        PropertyName = property.Name,
                        OldValue = oldValue,
                        NewValue = newValue
                    });
                }
            }

            return logs;
        }
    }
}
