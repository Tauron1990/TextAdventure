using Adventure.GameEngine.Builder;

namespace Adventure.GameEngine.BuilderAlt.RoomData
{
    public interface IRoomFactory
    {
        string Name { get; }

        RoomBuilder Apply(RoomBuilder builder, GameConfiguration gameConfiguration);
    }
}