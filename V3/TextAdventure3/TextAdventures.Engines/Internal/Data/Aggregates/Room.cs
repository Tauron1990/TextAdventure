using System;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Aggregates.Snapshot.Strategies;
using Akkatecture.Commands;
using Akkatecture.Specifications.Provided;
using TextAdventures.Builder.Data;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data.Commands;
using TextAdventures.Engine.Internal.Data.Events;

namespace TextAdventures.Engine.Internal.Data.Aggregates
{
    public sealed class RoomManager : AggregateManager<Room, RoomId, Command<Room, RoomId>>
    {
        public static readonly Guid RoomNamespace = new Guid("497C671A-F2F8-4BCE-BF84-5C4692B8DFBD");
    }

    public sealed class RoomState : GameState<RoomState, Room, RoomId>,
        IApply<RoomCreatedEvent>
    {
        public Name Name { get; set; } = new Name("");

        public void Apply(RoomCreatedEvent aggregateEvent) => Name = aggregateEvent.Name;

        public override void Hydrate(RoomState aggregateSnapshot) => Name = aggregateSnapshot.Name;
    }

    public sealed class Room : GameAggregate<Room, RoomId, RoomState>,
        IExecute<CreateRoomCommand>
    {
        public Room(RoomId id) 
            : base(id)
        {
        }

        protected override void SetSnapshotStrategy(ISnapshotStrategy snapshotStrategy)
        {
            base.SetSnapshotStrategy(new SnapshotAlwaysStrategy());
        }

        public bool Execute(CreateRoomCommand command)
        {
            if (new AggregateIsNewSpecification().IsSatisfiedBy(this)) 
                Emit(new RoomCreatedEvent(command.Name));

            return true;
        }
    }
}