using System;

namespace Adventure.GameEngine.Builder.CommandData
{
    public sealed class CommandId : IEquatable<CommandId>
    {
        private readonly int _id;

        public CommandId(int id)
            => _id = id;

        public bool Equals(CommandId? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return _id == other._id;
        }

        public override bool Equals(object? obj)
            => ReferenceEquals(this, obj) || obj is CommandId other && Equals(other);

        public override int GetHashCode()
            => _id;

        public static bool operator ==(CommandId? left, CommandId? right)
            => Equals(left, right);

        public static bool operator !=(CommandId? left, CommandId? right)
            => !Equals(left, right);
    }
}