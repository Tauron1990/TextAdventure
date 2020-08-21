using Adventure.GameEngine.Commands;
using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    public sealed class TravelProcessor : CommandProcessor<TravelCommand>
    {
        public TravelProcessor(IEventSystem eventSystem) 
            : base(eventSystem)
        {
        }

        protected override void ProcessCommand(TravelCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}