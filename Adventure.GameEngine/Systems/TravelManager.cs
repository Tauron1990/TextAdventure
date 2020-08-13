using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Systems;

namespace Adventure.GameEngine.Systems
{
    public sealed class TravelManager : IManualSystem, ISetupSystem, ITeardownSystem
    {
        private readonly IEventSystem _system;
        private readonly Dictionary<IEntity, IDisposable> _subscriptions;

        private readonly CompositeDisposable _compositeDisposable;

        public IGroup Group { get; } = new Group(typeof(RoomData), typeof(Room));

        public void Teardown(IEntity entity)
        {
        }

        public void Setup(IEntity entity)
        {
        }

        public TravelManager(IEventSystem system)
        {
            _system = system;

            _compositeDisposable = new CompositeDisposable
            {
                
            };

            
        }

        public void StartSystem(IObservableGroup observableGroup)
        {
            
        }

        public void StopSystem(IObservableGroup observableGroup)
        {

        }

        private sealed class ActionDispose : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}