using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Adventure.GameEngine.Core.Persistence;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core
{
    [PublicAPI]
    public sealed class LazyString : IPersitable, IEquatable<LazyString>
    {
        public ImmutableList<StringParameter> Parameter { get; private set; } = ImmutableList<StringParameter>.Empty;

        public string Text { get; private set; }

        private LazyString(string text, ImmutableList<StringParameter> parameter)
        {
            Text = text;
            Parameter = parameter;
        }

        private LazyString(string text)
            => Text = text;

        private LazyString()
            => Text = string.Empty;

        public static LazyString New()
            => new LazyString();

        public static LazyString New(string text)
            => new LazyString(text);

        public LazyString AddParameters(params string[] data)
        {
            return new LazyString(Text, Parameter.AddRange(data.Select(StringParameter.FromText)));
        }

        public LazyString AddParameters(params StringParameter[] data)
        {
            return new LazyString(Text, Parameter.AddRange(data));
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
            Parameter = BinaryHelper.ReadImmutableList<StringParameter>(reader);
        }

        public bool Equals(LazyString? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (other.Parameter.Count != Parameter.Count)
                return false;

            for (int i = 0; i < Parameter.Count; i++)
            {
                if (!Parameter[i].Equals(other.Parameter[i]))
                    return false;
            }

            return Text == other.Text;
        }

        public override bool Equals(object? obj)
            => ReferenceEquals(this, obj) || obj is LazyString other && Equals(other);

        public override int GetHashCode()
        {
            var current = Text.GetHashCode();

            return Parameter.Aggregate(current, (current1, parameter) => HashCode.Combine(current1, parameter.GetHashCode()));
        }

        public static bool operator ==(LazyString? left, LazyString? right)
            => Equals(left, right);

        public static bool operator !=(LazyString? left, LazyString? right)
            => !Equals(left, right);

        public static implicit operator LazyString(string text)
            => New(text);
    }
}