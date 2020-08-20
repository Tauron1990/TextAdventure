using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
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
        private readonly List<string> _genericCommands = new List<string>();
        private readonly CompositeDisposable _subscriptions;

        public IGroup Group { get; } = new Group(typeof(Room), typeof(RoomCommands));
        
        public CommandListUpdater(IEventSystem eventSystem, Game game)
        {
            _eventSystem = eventSystem;

            var group = game.ObservableGroupManager.GetObservableGroup(new Group(typeof(GenericCommandDescription)));

            _subscriptions = new CompositeDisposable
            {
                group.OnEntityAdded.Select(e => e.GetComponent<GenericCommandDescription>().Name).Subscribe(s => _genericCommands.Add(s)),
                group.OnEntityRemoving.Select(e => e.GetComponent<GenericCommandDescription>().Name).Subscribe(s => _genericCommands.Remove(s))
            };
        }

        public IObservable<IEntity> ReactToEntity(IEntity entity) 
            => entity.GetComponent<RoomData>().IsPlayerIn.Where(v => v).Select(v => entity);

        public void Process(IEntity entity) 
            => _eventSystem.Publish(new UpdateCommandList(entity.GetComponent<RoomCommands>().ProcessedCommands.Concat(_genericCommands)));

        public void Dispose() 
            => _subscriptions.Dispose();
    }
}