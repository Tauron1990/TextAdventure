using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace TextAdventures.Engine.Projection.Base
{
    public abstract class ProjectionBase<TId> : IProjectorData<TId>
    {
        private readonly ConcurrentDictionary<string, object?> _valueStore = new();

        public TId Id
        {
            get => GetItem(default(TId)!);
            set => SetItem(value);
        }

        protected void SetItem(object? item, [CallerMemberName] string name = null!) => _valueStore[name] = item;

        protected TObject GetItem<TObject>(Func<TObject> def, [CallerMemberName] string name = null!)
            => (TObject) (_valueStore.GetOrAdd(name, (s, v) => v(), def) ?? def!);

        protected TObject GetItem<TObject>(TObject def, [CallerMemberName] string name = null!)
            => (TObject) (_valueStore.GetOrAdd(name, (s, v) => v, def) ?? def!);
    }
}