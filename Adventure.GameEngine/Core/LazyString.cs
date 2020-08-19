using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adventure.GameEngine.Persistence;
using Adventure.Utilities.Interfaces;

namespace Adventure.GameEngine.Core
{
    public sealed class LazyString : IPersitable
    {
        public List<StringParameter> Parameter { get; private set; }

        public string Text { get; private set; }

        public LazyString(string text)
        {
            Parameter = new List<StringParameter>();
            Text = text;
        }

        public LazyString()
        {
            Parameter = new List<StringParameter>();
            Text = string.Empty;
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

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Text);
            BinaryHelper.WriteList(Parameter, writer);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Text = reader.ReadString();
            Parameter = BinaryHelper.ReadList<StringParameter>(reader);
        }
    }
}