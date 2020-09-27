using Akkatecture.Aggregates;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Processors.Commands
{
    public sealed class CommandLayerUpdatedEvent : AggregateEvent<CommandTracker, CommandTrackerId>
    {
        public RoomId Room { get; }

        public CommandLayer?[] Layers { get; }

        public CommandLayerUpdate Update { get; }

        public CommandLayerUpdatedEvent(RoomId room, CommandLayer?[] layers, CommandLayerUpdate update)
        {
            Room = room;
            Layers = layers;
            Update = update;
        }
    }
}