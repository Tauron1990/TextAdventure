using System;

namespace Adventure.GameEngine.BuilderAlt.ItemData
{
    public sealed class ItemId : IEquatable<ItemId>
    {
        private readonly string _id;

        public ItemId(string id)
            => _id = id;

        public bool Equals(ItemId? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return _id == other._id;
        }

        public override bool Equals(object? obj)
            => ReferenceEquals(this, obj) || obj is ItemId other && Equals(other);

        public override int GetHashCode()
            => _id.GetHashCode();

        public static bool operator ==(ItemId? left, ItemId? right)
            => Equals(left, right);

        public static bool operator !=(ItemId? left, ItemId? right)
            => !Equals(left, right);
    }
}