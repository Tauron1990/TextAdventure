using System.Threading;
using Adventure.GameEngine.Events;
using EcsRx.Events;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.ReactiveSystems.Custom;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class UpdateTimer : EventReactionSystem<MapBuild>
    {
        private Timer? _timer;

        public UpdateTimer(IEventSystem eventSystem) : base(eventSystem)
        {
        }

        public override void StartSystem(IObservableGroup observableGroup)
        {
            _timer = new Timer(SendUpdate);
            base.StartSystem(observableGroup);
        }

        public override void StopSystem(IObservableGroup observableGroup)
        {
            _timer?.Dispose();
            base.StopSystem(observableGroup);
        }

        private void SendUpdate(object state) 
            => EventSystem.Publish(GameConsts.UpdateCommand);

        public override void EventTriggered(MapBuild eventData) 
            => _timer?.Change(1000, 1000);
    }
}