using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data.Actor
{
    public enum Player
    {
        Special,
        Npc,
        Player
    }

    public sealed class PlayerType : SingleValueObject<Player>
    {
        public static readonly PlayerType Default = new PlayerType(Player.Special);

        public PlayerType(Player value) 
            : base(value)
        {
        }
    }
}