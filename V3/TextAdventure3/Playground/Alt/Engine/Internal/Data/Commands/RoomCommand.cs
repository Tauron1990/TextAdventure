using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public abstract class RoomCommand : GameCommand<RoomCommand, Room, RoomId>
    {
        protected RoomCommand(RoomId aggregateId)
            : base(aggregateId) { }
    }
}