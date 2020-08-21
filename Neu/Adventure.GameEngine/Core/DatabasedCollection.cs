using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events.Collections;
using EcsRx.Groups;
using EcsRx.MicroRx.Disposables;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.Computeds.Collections;
using EcsRx.ReactiveData;
using EcsRx.ReactiveData.Dictionaries;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core
{
    public abstract class DynamicValue<TValue> : IComputed<TValue>, IReactiveProperty<TValue>
    {
        private IReactiveProperty<TValue> _value = new ReactiveProperty<TValue>();

        public IDisposable Subscribe(IObserver<TValue> observer) =>
            _value.Subscribe(observer);

        public TValue Value
        {
            get => _value.Value;
            set => _value.Value = value;
        }

        public bool HasValue => _value.HasValue;

        public void Dispose() =>
            _value.Dispose();
    }

    [PublicAPI]
    public abstract class DataBasedCollection<TData> : IComputedCollection<TData>, IDisposable
    {
        private readonly ReactiveDictionary<IEntity, TData> _entrys = new ReactiveDictionary<IEntity, TData>();
        private readonly CompositeDisposable _subscriptions;
        private readonly Dictionary<IEntity, IDisposable> _trackedEntitys = new Dictionary<IEntity, IDisposable>();

        protected DataBasedCollection(IObservableGroupManager collection, IGroup groupInfo)
        {
            var group = collection.GetObservableGroup(groupInfo);
            _subscriptions = new CompositeDisposable
            {
                group.OnEntityAdded.Subscribe(RunAdd),
                group.OnEntityRemoving.Subscribe(RunRemove)
            };
        }

        public IDisposable Subscribe(IObserver<IEnumerable<TData>> observer)
        {
            return _entrys
                .ObserveCountChanged()
                .Select(_ => _entrys.Values)
                .Subscribe(observer);
        }

        public IEnumerable<TData> Value => _entrys.Values;

        public IEnumerator<TData> GetEnumerator() 
            => _entrys.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public TData this[int index] 
            => _entrys.ElementAt(index).Value;

        public int Count => _entrys.Count;

        public IObservable<CollectionElementChangedEvent<TData>> OnAdded
        {
            get
            {
                return _entrys
                    .ObserveAdd()
                    .Select(d => new CollectionElementChangedEvent<TData>
                    {
                        Index = _entrys.Count - 1,
                        NewValue = d.Value
                    });
            }
        }

        public IObservable<CollectionElementChangedEvent<TData>> OnRemoved
        {
            get
            {
                return _entrys
                    .ObserveRemove()
                    .Select(d => new CollectionElementChangedEvent<TData>
                    {
                        OldValue = d.Value
                    });
            }
        }

        public IObservable<CollectionElementChangedEvent<TData>> OnUpdated
        {
            get
            {
                return _entrys
                    .ObserveReplace()
                    .Select(d => new CollectionElementChangedEvent<TData>
                    {
                        Index = GetIndex(d.Key),
                        NewValue = d.NewValue,
                        OldValue = d.OldValue
                    });
            }
        }

        public void Dispose()
        {
            _entrys.Dispose();
            _subscriptions.Dispose();
            foreach (var trackedEntity in _trackedEntitys)
                trackedEntity.Value.Dispose();
            _trackedEntitys.Clear();
        }

        protected virtual void AddData(IEntity entity)
            => _entrys.Add(entity, Transform(entity));

        protected void RunRemove(IEntity entity)
        {
            if (_trackedEntitys.Remove(entity, out var subscription))
                subscription.Dispose();
            _entrys.Remove(entity);
        }

        protected void RunAdd(IEntity entity)
        {
            if (_trackedEntitys.ContainsKey(entity)) return;

            _trackedEntitys.Add(entity,
                AddWhen(entity).Subscribe(b => HandleAdd(b, entity)));
        }

        private void HandleAdd(bool when, IEntity entity)
        {
            if (when)
            {
                if (_entrys.ContainsKey(entity)) return;
                AddData(entity);
            }
            else
                _entrys.Remove(entity);
        }

        protected abstract TData Transform(IEntity entity);

        protected abstract IObservable<bool> AddWhen(IEntity entity);

        private int GetIndex(IEntity key)
        {
            for (var i = 0; i < _entrys.Count; i++)
            {
                if (_entrys.ElementAt(i).Key == key)
                    return i;
            }

            return -1;
        }
    }
}