using System;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Engine.Internal;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands
{
    public abstract class GameCommand<TThis, TAggregate, TIdentity> : Command<TAggregate, TIdentity>, ILogCommand
        where TThis : GameCommand<TThis, TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        protected GameCommand(TIdentity aggregateId)
            : base(aggregateId, CommandIdProvider.Current) { }

        public CommandCallData CallData { get; set; } = new(CallType.Internal);

        public string Name { get; set; } = string.Empty;

        Type IGameCommand.Target => typeof(TThis);
    }
}