using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Models
{
    public class InsightField : IEquatable<InsightField>
    {
        public int? Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public IEnumerable<InsightFieldAttribute> Attributes { get; set; }

        public bool Equals(InsightField other)
        {
            return Id == other.Id && Key == other.Key && Name == other.Name;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Key, Name);
        }
    }

    public class InsightFieldAttribute
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public IEnumerable<string> DisplayValues { get; set; }
    }
}
