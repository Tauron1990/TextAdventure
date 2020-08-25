using Adventure.GameEngine.BuilderAlt;

namespace Adventure.GameEngine.Builder.RoomData
{
    public interface IRoomFactory
    {
        string Name { get; }

        RoomBuilder Apply(RoomBuilder builder, GameConfiguration gameConfiguration);
    }
}