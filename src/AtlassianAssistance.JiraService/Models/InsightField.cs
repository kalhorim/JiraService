﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianAssistance.JiraService.Models
{
    public class InsightField : IEquatable<InsightField>
    {
        public string FieldTypeId { get; set; }
        public int? Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public IEnumerable<InsightFieldAttribute> Attributes { get; set; }
        internal JArray AttributesObject { get; set; }

        public bool Equals(InsightField other)
        {
            return Id == other.Id && Key == other.Key && Name == other.Name;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + Key.GetHashCode();
                hash = hash * 23 + Name.GetHashCode();
                return hash;
            }
        }
    }

    public class InsightFieldAttribute
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public IEnumerable<Object> Values { get; set; }
        public IEnumerable<string> DisplayValues { get; set; }
    }
}
