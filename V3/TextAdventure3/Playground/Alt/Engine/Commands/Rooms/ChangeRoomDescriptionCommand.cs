using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands.Rooms
{
    public sealed class ChangeRoomDescriptionCommand : RoomCommand
    {
        public ChangeRoomDescriptionCommand(RoomId aggregateId, Description description, bool isDeatil) : base(aggregateId)
        {
            Description = description;
            IsDeatil    = isDeatil;
        }

        public Description Description { get; }

        public bool IsDeatil { get; }
    }
}