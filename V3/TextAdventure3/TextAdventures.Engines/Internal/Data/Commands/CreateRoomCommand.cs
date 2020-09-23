using Akkatecture.Commands;
using TextAdventures.Builder.Data;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public sealed class CreateRoomCommand : Command<Room, RoomId>
    {
        public Name Name { get; }

        public CreateRoomCommand(Name name)
            : base(RoomId.NewDeterministic(RoomManager.RoomNamespace, name.Value))
            => Name = name;
    }
}