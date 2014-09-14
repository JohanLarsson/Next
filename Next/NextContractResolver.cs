namespace Next
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using System.Reflection;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class NextContractResolver : DefaultContractResolver
    {
        private readonly string[] _prefixable = new[] { "timestamp", "volume", "size", "buying", "selling", "status" };
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            string propertyName = prop.PropertyName;
            if (_prefixable.Any(x => this.TryPrefix(propertyName, x, out propertyName)))
            {
                prop.PropertyName = propertyName;
            }

            return prop;
        }

        private bool TryPrefix(string input,string prefix, out string prefixed)
        {
            if (input.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) > 0)
            {
                prefixed = Regex.Replace(input, prefix, string.Concat("_", prefix.ToLower()), RegexOptions.IgnoreCase);
                return true;
            }
            prefixed = input;
            return false;
        }
    }
}
