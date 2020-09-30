using System;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using TextAdventures.Builder.Data.Command;

namespace TextAdventures.Engine.Commands
{
    public abstract class GameCommand<TThis, TAggregate, TIdentity> : Command<TAggregate, TIdentity>, IGameCommand, ICommandMetadata
        where TThis : GameCommand<TThis, TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        public string Name { get; set; } = string.Empty;
        
        Type IGameCommand.Target => typeof(TThis);

        protected GameCommand(TIdentity aggregateId) 
            : base(aggregateId)
        {
        }

        protected GameCommand(TIdentity aggregateId, CommandId sourceId) 
            : base(aggregateId, sourceId)
        { 
        }
    }
}