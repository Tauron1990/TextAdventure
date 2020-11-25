using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data.Rooms
{
    public sealed class Direction : SingleValueObject<string>
    {
        public static readonly Direction South = new("South");

        public static readonly Direction SouthEast = new("SouthEast");

        public static readonly Direction SouthWest = new("SouthWest");

        public static readonly Direction North = new("North");

        public static readonly Direction NorthWest = new("NorthWest");

        public static readonly Direction NorthEast = new("NorthEast");

        public static readonly Direction East = new("East");

        public static readonly Direction West = new("West");

        public Direction(string value) : base(value) { }
    }
}