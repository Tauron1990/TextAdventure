using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data.Rooms
{
    public sealed class Direction : SingleValueObject<string>
    {
        public Direction(string value) : base(value)
        {
        }

        public static readonly Direction South = new Direction("South");

        public static readonly Direction SouthEast = new Direction("SouthEast");

        public static readonly Direction SouthWest = new Direction("SouthWest");

        public static readonly Direction North = new Direction("North");

        public static readonly Direction NorthWest = new Direction("NorthWest");

        public static readonly Direction NorthEast = new Direction("NorthEast");

        public static readonly Direction East = new Direction("East");

        public static readonly Direction West = new Direction("West");
    }
}