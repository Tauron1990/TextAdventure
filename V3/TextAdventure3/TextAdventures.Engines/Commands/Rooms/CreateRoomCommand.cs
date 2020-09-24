using TextAdventures.Builder.Data;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data.Aggregates;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands.Rooms
{
    public sealed class CreateRoomCommand : RoomCommand
    {
        public Name Name { get; }

        public CreateRoomCommand(Name name)
            : base(RoomId.NewDeterministic(RoomManager.RoomNamespace, name.Value))
            => Name = name;
    }
}