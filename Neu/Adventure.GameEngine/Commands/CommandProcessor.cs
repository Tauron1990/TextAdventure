using EcsRx.Events;
using EcsRx.Plugins.ReactiveSystems.Custom;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    [PublicAPI]
    public abstract class CommandProcessor<TCommand> : EventReactionSystem<TCommand>
        where TCommand : Command<TCommand>
    {
        //TODO Provide Importent Propertys

        protected CommandProcessor(IEventSystem eventSystem) 
            : base(eventSystem)
        {
        }

        public sealed override void EventTriggered(TCommand eventData)
            => ProcessCommand(eventData);

        protected abstract void ProcessCommand(TCommand command);
    }
}