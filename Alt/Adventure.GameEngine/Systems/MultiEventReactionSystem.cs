using System;
using System.Reactive.Disposables;
using EcsRx.Events;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;

namespace Adventure.GameEngine.Systems
{
    public abstract class MultiEventReactionSystem : IManualSystem
    {
        private CompositeDisposable _disposable = new CompositeDisposable();

        public IEventSystem EventSystem { get; }

        public IObservableGroup ObservableGroup { get; internal set; } = null!;

        protected MultiEventReactionSystem(IEventSystem eventSystem) => EventSystem = eventSystem;

        public virtual IGroup Group { get; } = new EmptyGroup();

        protected abstract void Init();

        protected void Receive<T>(Action<T> handler) => _disposable.Add(EventSystem.Receive<T>().Subscribe(handler));

        public virtual void StartSystem(IObservableGroup observableGroup)
        {
            ObservableGroup = observableGroup;
            Init();
            _disposable = new CompositeDisposable();
        }

        public virtual void StopSystem(IObservableGroup observableGroup) 
            => _disposable.Dispose();
    }
}