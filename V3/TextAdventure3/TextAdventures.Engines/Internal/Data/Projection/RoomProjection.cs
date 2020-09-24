using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Internal.Data.Projection
{
    public class RoomProjection : IProjectorData<RoomId>
    {
        public RoomId Id { get; set; } = new RoomId("");
    }
}