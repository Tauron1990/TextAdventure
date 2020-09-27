using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands.Rooms
{
    public sealed class CreateRoomCommand : RoomCommand
    {
        public Name Name { get; }

        public Doorway[] Doorways { get; }

        public CreateRoomCommand(Name name, Doorway[] doorways)
            : base(RoomId.FromName(name))
        {
            Name = name;
            Doorways = doorways;
        }
    }
}