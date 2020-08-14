using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.Utilities.Interfaces;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Core
{
    public sealed class LazyString
    {
        public List<StringParameter> Parameter { get; }

        public string Text { get; }

        public LazyString(string text)
        {
            Parameter = new List<StringParameter>();
            Text = text;
        }

        [JsonConstructor]
        public LazyString(List<StringParameter> parameter, string text)
        {
            Parameter = parameter;
            Text = text;
        }

        public LazyString AddParameters(params string[] data)
        {
            Parameter.AddRange(data.Select(StringParameter.FromText));
            return this;
        }

        public LazyString AddParameters(params StringParameter[] data)
        {
            Parameter.AddRange(data);
            return this;
        }

        public string Format(IContentManagement management)
        {
            var realText = management.RetrieveContentItem(Text, Text);

            return string.Format(realText, Parameter.Select(s => s.Format(management)).Cast<object>().ToArray());
        }
    }
}