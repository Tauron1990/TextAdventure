using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class EventData<TData> : EventData
    {
        private readonly TData _data;

        public EventData(TData data)
            => _data = data;

        public override object Data => _data!;

        internal override void Dispatch(IEventSystem eventSystem)
            => eventSystem.Publish(_data);
    }

    [PublicAPI]
    public abstract class EventData
    {
        public abstract object Data { get; }

        internal abstract void Dispatch(IEventSystem eventSystem);
    }
}