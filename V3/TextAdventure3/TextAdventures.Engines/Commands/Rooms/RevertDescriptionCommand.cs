using JetBrains.Annotations;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands.Rooms
{
    public sealed class RevertDescriptionCommand : RoomCommand
    {
        public RevertDescriptionCommand(RoomId aggregateId) 
            : base(aggregateId)
        {
        }
    }
}