using System;
using System.IO;
using Adventure.GameEngine.Core.Persistence;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core
{
    [PublicAPI]
    public sealed class StringParameter : IPersitable, IEquatable<StringParameter>
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

        public bool Equals(StringParameter? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Resolve == other.Resolve && Text == other.Text;
        }

        public override bool Equals(object? obj)
            => ReferenceEquals(this, obj) || obj is StringParameter other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Resolve, Text);

        public static bool operator ==(StringParameter? left, StringParameter? right)
            => Equals(left, right);

        public static bool operator !=(StringParameter? left, StringParameter? right)
            => !Equals(left, right);
    }
}