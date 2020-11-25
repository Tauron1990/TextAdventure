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
        public static readonly CommandLogId Id = With(new Guid("12C06230-00A4-45C6-AB6D-A151C7C5004B"));

        [JsonConstructor]
        public CommandLogId(string value)
            : base(value) { }
    }

    public sealed class CommandLogStade : GameState<CommandLogStade, CommandLog, CommandLogId>,
                                          IApply<CommandToLogAddedEvent>
    {
        public LimitedList<ILogCommand> Commands { get; set; } = new(100);

        public void Apply(CommandToLogAddedEvent aggregateEvent)
            => Commands.Add(aggregateEvent.Command);

        public override void Hydrate(CommandLogStade aggregateSnapshot)
            => Commands = aggregateSnapshot.Commands;
    }

    public sealed class CommandLog : GameAggregate<CommandLog, CommandLogId, CommandLogStade>,
                                     IHandle<CommandLogCommand>
    {
        public CommandLog(CommandLogId id)
            : base(id) { }

        public void Handle(CommandLogCommand message)
            => Emit(new CommandToLogAddedEvent(message.Command));
    }

    public sealed class CommandLogManager : AggregateManager<CommandLog, CommandLogId, CommandLogCommand> { }
}