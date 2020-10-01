using System;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Newtonsoft.Json;
using Tauron.Application;
using TextAdventures.Engine.Internal.Data.Commands;
using TextAdventures.Engine.Internal.Data.Events;

namespace TextAdventures.Engine.Internal.Data.Aggregates
{
    public sealed class CommandLogId : Identity<CommandLogId>
    {
        [JsonConstructor]
        public CommandLogId(string value) 
            : base(value)
        {
        }

        public static readonly CommandLogId Id = With(new Guid("12C06230-00A4-45C6-AB6D-A151C7C5004B"));
    }

    public sealed class CommandLogStade : GameState<CommandLogStade, CommandLog, CommandLogId>,
        IApply<CommandToLogAddedEvent>
    {

        public LimitedList<ILogCommand> Commands { get; set; } = new LimitedList<ILogCommand>(100);

        public override void Hydrate(CommandLogStade aggregateSnapshot) 
            => Commands = aggregateSnapshot.Commands;

        public void Apply(CommandToLogAddedEvent aggregateEvent) 
            => Commands.Add(aggregateEvent.Command);
    }

    public sealed class CommandLog : GameAggregate<CommandLog, CommandLogId, CommandLogStade>,
        IHandle<CommandLogCommand>
    {
        public CommandLog(CommandLogId id) 
            : base(id)
        { }

        public void Handle(CommandLogCommand message) 
            => Emit(new CommandToLogAddedEvent(message.Command));
    }

    public sealed class CommandLogManager : AggregateManager<CommandLog, CommandLogId, CommandLogCommand>
    {

    }
}