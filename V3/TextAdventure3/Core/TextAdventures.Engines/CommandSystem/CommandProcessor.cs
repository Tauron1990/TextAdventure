using System;
using JetBrains.Annotations;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.CommandSystem
{
    public delegate void RunCommand<in TCommand, in TComponent>(ICommandContext<TComponent> context, TCommand command);

    public abstract class CommandProcessorBase
    {
        public abstract Type Component { get; }

        public abstract bool CanProcess(object component);

        public abstract void Run(GameCore core, object component, GameObject gameObject, IGameCommand command);
    }

    public abstract record RegisterCommandProcessorBase
    {
        public abstract Type CommandType { get; }

        public abstract CommandProcessorBase CreateProcessor();
    }

    public sealed record RegisterCommandProcessor<TCommand, TComponent>
        (RunCommand<TCommand, TComponent> Processor) : RegisterCommandProcessorBase
        where TCommand : IGameCommand
    {
        public override Type CommandType => typeof(TCommand);

        public override CommandProcessorBase CreateProcessor() => new ProcessorImpl(Processor);

        private sealed class ProcessorImpl : CommandProcessorBase
        {
            private readonly RunCommand<TCommand, TComponent> _processor;

            public ProcessorImpl(RunCommand<TCommand, TComponent> processor)
                => _processor = processor;

            public override Type Component => typeof(TComponent);

            public override bool CanProcess(object component) => component is TComponent;

            public override void Run(GameCore core, object component, GameObject gameObject, IGameCommand command)
                => _processor(new Context(core, (TComponent) component, gameObject), (TCommand) command);

            private sealed class Context : ICommandContext<TComponent>
            {
                public Context(GameCore game, TComponent component, GameObject o)
                {
                    Game = game;
                    Component = component;
                    Object = o;
                }

                public GameCore Game { get; }
                public TComponent Component { get; }
                public GameObject Object { get; }

                public void EmitEvents(params object[] events)
                {
                    var comp = Component as ComponentBase;

                    foreach (var @event in events)
                    {
                        comp?.ApplyEvent(@event);
                        Game.EventDispatcher.Send(@event);
                    }
                }
            }
        }
    }

    public interface ICommandProcessorRegistrar
    {
        RegisterCommandProcessorBase CreateRegistration();
    }

    public abstract class CommandProcessor<TCommand, TComponent> : ICommandProcessorRegistrar
        where TCommand : IGameCommand
    {
        RegisterCommandProcessorBase ICommandProcessorRegistrar.CreateRegistration()
        {
            return CommandProcessor.RegistrationFor<TCommand, TComponent>(RunCommand);
        }

        protected abstract void RunCommand(ICommandContext<TComponent> commandContext, TCommand command);
    }

    [PublicAPI]
    public static class CommandProcessor
    {
        public static RegisterCommandProcessorBase RegistrationFor<TCommand, TComponent>(
            RunCommand<TCommand, TComponent> processor)
            where TCommand : IGameCommand
            => new RegisterCommandProcessor<TCommand, TComponent>(processor);

        public static RegisterCommandProcessorBase RegistrationFor<TType>()
            where TType : ICommandProcessorRegistrar, new() => new TType().CreateRegistration();
    }
}