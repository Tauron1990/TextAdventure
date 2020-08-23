using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Systems.Components;
using Adventure.GameEngine.Systems.Events;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class CommandListUpdater : IReactToEntitySystem, IDisposable
    {
        private readonly IEventSystem _eventSystem;
        private readonly Dictionary<LazyString, Command> _genericCommands = new Dictionary<LazyString, Command>();
        private readonly CompositeDisposable _subscriptions;

        public IGroup Group { get; } = new Group(typeof(Room), typeof(RoomCommands));

        public CommandListUpdater(IEventSystem eventSystem, IObservableGroupManager manager)
        {
            _eventSystem = eventSystem;
            var currentRoom = new CurrentRoom(manager);

            var group = manager.GetObservableGroup(new Group(typeof(GenericCommandDescription), typeof(GenericCommand)));

            _subscriptions = new CompositeDisposable
            {
                group.OnEntityAdded.Select(ReadData).Subscribe(s => _genericCommands.Add(LazyString.New(s.Name), s.Command)),
                group.OnEntityRemoving.Select(ReadData).Subscribe(s => _genericCommands.Remove(LazyString.New(s.Name))),
                currentRoom,
                eventSystem.Receive<ForceUpdateCommandList>().Subscribe(_ =>
                {
                    eventSystem.Publish(
                        new UpdateCommandList(
                            currentRoom.Commands.Commands
                                .Concat(_genericCommands).Select(p => (p.Key, p.Value))));
                })
            };
        }

        private static (string Name, Command Command) ReadData(IEntity entity)
        {
            return (
                entity.GetComponent<GenericCommandDescription>().Name,
                entity.GetComponent<GenericCommand>().Command
            );
        }

        public IObservable<IEntity> ReactToEntity(IEntity entity)
            => entity.GetComponent<RoomData>().IsPlayerIn.Where(v => v).Select(v => entity);

        public void Process(IEntity entity)
            => _eventSystem.Publish(
                new UpdateCommandList(
                    entity.GetComponent<RoomCommands>().Commands
                        .Concat(_genericCommands).Select(p => (p.Key, p.Value))));

        public void Dispose()
            => _subscriptions.Dispose();
    }
}