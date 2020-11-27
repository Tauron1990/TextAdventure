using System;
using Akka.Actor;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.CommandSystem
{
    public delegate void RunCommand<in TCommand, in TComponent>(ICommandContext<TComponent> context, TCommand command);

    public abstract class CommandProcessorBase
    {
        public abstract Type Component { get; }

        public abstract bool CanProcess(object component);

        public abstract void Run(GameCore core, object component, GameObject gameObject);
    }

    public abstract record RegisterCommandProcessorBase
    {
        public abstract CommandProcessorBase CreateProcessor();
    }

    public sealed record RegisterCommandProcessor<TCommand, TComponent>(RunCommand<TCommand, TCommand> Processor) : RegisterCommandProcessorBase 
    {
        public override CommandProcessorBase CreateProcessor()
        {

        }

        private sealed class ProcessorImpl : CommandProcessorBase
        {
            private readonly RunCommand<TCommand, TCommand> _processor;

            public ProcessorImpl(RunCommand<TCommand, TCommand> processor) 
                => _processor = processor;

            public override Type Component => typeof(TCommand);
         
            public override bool CanProcess(object component) => ;

            public override void Run(GameCore core, object component, GameObject gameObject) { }
        }
    }
}