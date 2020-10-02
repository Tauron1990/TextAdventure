using System;
using System.Collections.Generic;
using System.Linq;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Aggregates.Snapshot.Strategies;
using Akkatecture.Extensions;
using Akkatecture.Specifications.Provided;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Commands.Rooms;
using TextAdventures.Engine.Events.Rooms;
using TextAdventures.Engine.Internal.Data.Commands;
using TextAdventures.Engine.Internal.Data.Events;

namespace TextAdventures.Engine.Internal.Data.Aggregates
{
    public sealed class RoomManager : AggregateManager<Room, RoomId, RoomCommand>
    {

    }

    public sealed class RoomState : GameState<RoomState, Room, RoomId>,
        IApply<RoomCreatedEvent>, IApply<RoomCommandLayerRemovedEvent>, IApply<RoomDescriptionRevertEvent>,
        IApply<RoomCommandsAddedEvent>, IApply<RoomDescriptionChangedEvent>
    {
        public Name Name { get; set; } = Name.Default;

        public Doorway[] Doorways { get; set; } = Array.Empty<Doorway>();

        public List<CommandLayer> Layers { get; set; } = new List<CommandLayer>();

        public Stack<Description> Descriptions { get; } = new Stack<Description>();

        public Stack<Description> DetailDescriptions { get; } = new Stack<Description>();

        public override void Hydrate(RoomState aggregateSnapshot)
        {
            Name = aggregateSnapshot.Name;
            Doorways = aggregateSnapshot.Doorways;
            Layers.AddRange(aggregateSnapshot.Layers);

            foreach (var description in aggregateSnapshot.Descriptions) 
                Descriptions.Push(description);

            foreach (var detailDescription in aggregateSnapshot.DetailDescriptions) 
                DetailDescriptions.Push(detailDescription);
        }

        public void Apply(RoomCreatedEvent aggregateEvent)
        {
            Name = aggregateEvent.Name;
            Doorways = aggregateEvent.Doorways;
        }

        public void Apply(RoomCommandsAddedEvent aggregateEvent) 
            => Layers.AddRange(aggregateEvent.Layers);

        public void Apply(RoomCommandLayerRemovedEvent aggregateEvent)
        {
            var result = Layers.AsEnumerable().Reverse().FirstOrDefault(l => l.Name == aggregateEvent.Name);
            if (result != null)
                Layers.Remove(result);
        }

        public void Apply(RoomDescriptionChangedEvent aggregateEvent)
        {
            if(aggregateEvent.IsDetail)
                DetailDescriptions.Push(aggregateEvent.Description);
            else
                Descriptions.Push(aggregateEvent.Description);
        }

        public void Apply(RoomDescriptionRevertEvent aggregateEvent)
        {
            if (aggregateEvent.Detail)
                DetailDescriptions.Pop();
            else
                Descriptions.Pop();
        }
    }

    [AggregateName("Room")]
    public sealed class Room : GameAggregate<Room, RoomId, RoomState>,
        IExecute<CreateRoomCommand>, IExecute<RemoveCommandLayerCommand>,
        IExecute<AddCommandLayerCommand>
    {
        public Room(RoomId id) 
            : base(id) { }

        private bool IsActualRoom()
            => AggregateIsNewSpecification.Instance.Not().IsSatisfiedBy(this);

        public bool Execute(CreateRoomCommand command)
        {
            if (AggregateIsNewSpecification.Instance.IsSatisfiedBy(this)) 
            {
                EmitAll(
                    new RoomCreatedEvent(command.RoomName, command.AggregateId, command.Doorways),
                    new RoomDescriptionChangedEvent(command.Description, false),
                    new RoomDescriptionChangedEvent(command.DetailDescription, true),
                    new RoomCommandsAddedEvent(command.CommandLayers, command.AggregateId));
            }

            return true;
        }

        public bool Execute(AddCommandLayerCommand command)
        {
            if(IsActualRoom())
                Emit(new RoomCommandsAddedEvent(command.CommandLayers.ToArray(), Id));
            return true;
        }

        public bool Execute(RemoveCommandLayerCommand command)
        {
            if(IsActualRoom())
                Emit(new RoomCommandLayerRemovedEvent(Id, command.Name));
            return true;
        }
    }
}