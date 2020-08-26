using Adventure.GameEngine.Core;
using Adventure.GameEngine.Systems.Events;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Plugins.ReactiveSystems.Custom;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class GameEventSystem : EventReactionSystem<TriggerEvent>
    {
        private readonly CurrentRoom _currentRoom;

        public GameEventSystem(IEventSystem eventSystem, IObservableGroupManager manager) 
            : base(eventSystem)
            => _currentRoom = new CurrentRoom(manager);

        public override void EventTriggered(TriggerEvent eventData)
        {
            var events = _currentRoom.Events;

            if (events.Events.TryGetValue(eventData.Name, out var evt))
                evt.ForEach(e => e.Dispatch(EventSystem));
        }
    }
}