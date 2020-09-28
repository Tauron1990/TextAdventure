using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.Specifications.Provided;
using Tauron;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Events;
using TextAdventures.Engine.Internal.Data.Aggregates;
using TextAdventures.Engine.Internal.Data.Events;
using TextAdventures.Engine.Querys.Result;
using TextAdventures.Engine.Querys.Room;

namespace TextAdventures.Engine.Processors.Commands
{
    public sealed class CommandTrackerId : GameSagaId<CommandTrackerId>
    {
        public CommandTrackerId(string value)
            : base(value)
        {
        }
    }

    public sealed class CommandTrackerLocator : GameSagaLocator<CommandTrackerId>
    {
        public override CommandTrackerId LocateSaga(IDomainEvent domainEvent)
        {
            return domainEvent.GetAggregateEvent() switch
            {
                PlayerCreated evt => new CommandTrackerId($"CommandTracker-{evt.Actor}"),
                _ => throw new ArgumentException("No Supportet Event", nameof(domainEvent))
            };
        }
    }

    public sealed class CommandTrackerState : GameSageState<CommandTrackerState, CommandTracker, CommandTrackerId>,
                                              IApply<PlayerRecivedEvent>, IApply<CommandLayerUpdatedEvent>
    {
        public GameActorId Player { get; set; } = null!;

        public RoomId CurrentRoom { get; set; } = null!;

        public List<CommandLayer> CurrentCommands { get; set; } = new List<CommandLayer>();

        public override void Hydrate(CommandTrackerState aggregateSnapshot)
        {
            Player = aggregateSnapshot.Player;
            CurrentCommands = aggregateSnapshot.CurrentCommands;
            CurrentRoom = aggregateSnapshot.CurrentRoom;
        }

        public void Apply(PlayerRecivedEvent aggregateEvent)
            => Player = aggregateEvent.Player;

        public void Apply(CommandLayerUpdatedEvent aggregateEvent)
        {
            switch (aggregateEvent.Update.Value)
            {
                case CommandLayerUpdateValue.Replace:
                    CurrentRoom = aggregateEvent.Room;
                    CurrentCommands.Clear();
                    CurrentCommands.AddRange(aggregateEvent.Layers.Where(l => l != null)!);
                    break;
                case CommandLayerUpdateValue.Remove:
                    foreach (var toRemove in aggregateEvent.Layers)
                    {
                        var item = CurrentCommands.AsEnumerable().Reverse().FirstOrDefault(cl => cl == toRemove);
                        if(item == null)
                            continue;

                        CurrentCommands.Remove(item);
                    }
                    break;
                case CommandLayerUpdateValue.Add:
                    CurrentCommands.AddRange(aggregateEvent.Layers.Where(l => l != null)!);
                    break;
            }
        }
    }

    [AggregateName("CommandTracker")]
    public sealed class CommandTracker : GameSaga<CommandTracker, CommandTrackerId, CommandTrackerState>,
                                         ISagaIsStartedBy<GameActor, GameActorId, PlayerCreated>, ISagaHandles<Room, RoomId, RoomCommandLayerRemovedEvent>,
                                         ISagaHandlesAsync<GameActor, GameActorId, ActorRoomChanged>, ISagaHandles<Room, RoomId, RoomCommandsAddedEvent>,
                                         ISagaHandles<CommandTracker, CommandTrackerId, CommandLayerUpdatedEvent>, ISagaHandles<GameInfo, GameInfoId, GameLoaded>,
                                         ISagaHandles<GameInfo, GameInfoId, UpdateCommandEvent>

    {
        private readonly IActorRef _gameMaster;

        public CommandTracker(IActorRef gameMaster)
            => _gameMaster = gameMaster;

        public bool Handle(IDomainEvent<GameActor, GameActorId, PlayerCreated> domainEvent)
        {
            if (AggregateIsNewSpecification.Instance.IsSatisfiedBy(this))
                Emit(new PlayerRecivedEvent(domainEvent.AggregateEvent.Actor));
            return true;
        }

        public async Task HandleAsync(IDomainEvent<GameActor, GameActorId, ActorRoomChanged> domainEvent)
        {
            var actualEvent = domainEvent.AggregateEvent;
            if(IsNew || State.Player != actualEvent.Actor || State.Player == actualEvent.NewRoom) return;

            CommandLayerUpdatedEvent? evt = null;

            try
            {
                var query = await _gameMaster.Ask<QueryResult>(new QueryCommandLayers(actualEvent.NewRoom));
                if (query is QueryCompled compled)
                    evt = new CommandLayerUpdatedEvent(actualEvent.NewRoom, (CommandLayer[]) compled.Result, new CommandLayerUpdate(CommandLayerUpdateValue.Replace));
            }
            catch (Exception e)
            {
                Log.Error(e, "Error on Update Command Layers");
            }

            Emit(evt ?? new CommandLayerUpdatedEvent(actualEvent.NewRoom, Array.Empty<CommandLayer>(), new CommandLayerUpdate(CommandLayerUpdateValue.Replace)));
        }

        public bool Handle(IDomainEvent<Room, RoomId, RoomCommandLayerRemovedEvent> domainEvent)
        {
            if (IsNew || State.CurrentRoom != domainEvent.AggregateIdentity) return true;

            Emit(new CommandLayerUpdatedEvent(
                State.CurrentRoom, 
                new []{ State.CurrentCommands.AsEnumerable().FirstOrDefault(cl => cl.Name == domainEvent.AggregateEvent.Name) },
                new CommandLayerUpdate(CommandLayerUpdateValue.Remove)));

            return true;
        }

        public bool Handle(IDomainEvent<Room, RoomId, RoomCommandsAddedEvent> domainEvent)
        {
            if (IsNew || State.CurrentRoom != domainEvent.AggregateIdentity) return true;

            Emit(new CommandLayerUpdatedEvent(State.CurrentRoom, domainEvent.AggregateEvent.Layers, new CommandLayerUpdate(CommandLayerUpdateValue.Add)));
            return true;
        }

        public bool Handle(IDomainEvent<CommandTracker, CommandTrackerId, CommandLayerUpdatedEvent> domainEvent)
        {
            SendActuralLayers();
            return true;
        }

        private void SendActuralLayers()
        {
            var layers = new List<CommandLayer>();

            foreach (var command in State.CurrentCommands)
            {
                if (command.Exclusive && layers.Count == 0)
                {
                    layers.Add(command);
                    break;
                }
                if (command.Exclusive)
                    break;
                layers.Add(command);
            }

            var commands = new List<IGameCommand>();
            foreach (var layer in layers)
            {
                var command = layer.Command.FastCreateInstance();

                switch (command)
                {
                    case IGameCommand com:
                        commands.Add(com);
                        break;
                    case ICommandBuilder builder:
                        commands.AddRange(builder.Produce(layer.Metadata));
                        break;
                }
            }

            new CommandsUpdatedEvent(ImmutableList<IGameCommand>.Empty.AddRange(commands))
               .Publish(Context.System.EventStream);
        }

        public bool Handle(IDomainEvent<GameInfo, GameInfoId, GameLoaded> domainEvent)
        {
            SendActuralLayers();
            return true;
        }

        public bool Handle(IDomainEvent<GameInfo, GameInfoId, UpdateCommandEvent> domainEvent)
        {
            SendActuralLayers();
            return true;
        }
    }

    public sealed class CommandTrackerManager : GameSagaManger<CommandTracker, CommandTrackerId, CommandTrackerLocator>
    {
        public CommandTrackerManager(IActorRef gameMaster)
            : base(() => new CommandTracker(gameMaster))
        {

        }
    }
}