using Akkatecture.ValueObjects;
using JetBrains.Annotations;

namespace TextAdventures.Builder.Data.Actor
{
    [PublicAPI]
    public enum Player
    {
        Special,
        Npc,
        Player
    }

    [PublicAPI]
    public sealed class PlayerType : SingleValueObject<Player>
    {
        public static readonly PlayerType Default = new(Player.Special);

        public PlayerType(Player value)
            : base(value) { }
    }
}