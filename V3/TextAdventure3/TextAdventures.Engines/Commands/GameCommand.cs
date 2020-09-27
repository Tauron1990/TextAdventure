using System;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands
{
    public abstract class GameCommand<TThis, TAggregate, TIdentity> : Command<TAggregate, TIdentity>, IGameCommand
        where TThis : GameCommand<TThis, TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        Type IGameCommand.Target => typeof(TThis);

        protected GameCommand(TIdentity aggregateId) : base(aggregateId)
        {
        }

        protected GameCommand(TIdentity aggregateId, CommandId sourceId) : base(aggregateId, sourceId)
        {
        }
    }
}