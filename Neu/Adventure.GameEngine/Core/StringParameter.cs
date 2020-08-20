using System.IO;
using Adventure.GameEngine.Core.Persistence;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core
{
    [PublicAPI]
    public sealed class StringParameter : IPersitable
    {
        public bool Resolve { get; private set; }

        public string Text { get; private set; }

        public StringParameter(bool resolve, string text)
        {
            Resolve = resolve;
            Text = text;
        }

        public StringParameter()
        {
            Resolve = false;
            Text = string.Empty;
        }

        public string Format(IContentManagement management) 
            => Resolve ? management.RetrieveContentItem(Text, Text) : Text;

        public static StringParameter Resolved(string data)
            => new StringParameter(true, data);

        public static StringParameter FromText(string data)
            => new StringParameter(false, data);

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Resolve);
            writer.Write(Text);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Resolve = reader.ReadBoolean();
            Text = reader.ReadString();
        }
    }
}