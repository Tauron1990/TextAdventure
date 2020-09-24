using Akkatecture.Commands;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public abstract class RoomCommand : GameCommand<RoomCommand, Room, RoomId>
    {
        protected RoomCommand(RoomId aggregateId) 
            : base(aggregateId)
        {
        }

        protected RoomCommand(RoomId aggregateId, CommandId sourceId) 
            : base(aggregateId, sourceId)
        {
        }
    }
}