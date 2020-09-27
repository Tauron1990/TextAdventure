using System;
using Akka.Actor;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using TextAdventures.Builder.Commands;
using TextAdventures.Engine.Internal.Data;

namespace TextAdventures.Engine.Commands
{
    public sealed class NewAggregate<TManager, TAggregate, TId, TCommand> : INewAggregate
        where TManager : AggregateManager<TAggregate, TId, TCommand>, new()
        where TCommand : ICommand<TAggregate, TId>
        where TId : IIdentity 
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TId>
    {
        Type INewAggregate.Target => typeof(TCommand);

        Props INewAggregate.Props => Props.Create<TManager>();

        string INewAggregate.Name => typeof(TAggregate).Name;

        private NewAggregate()
        {
            
        }

        public static NewAggregate<TManager, TAggregate, TId, TCommand> Create()
            => new NewAggregate<TManager, TAggregate, TId, TCommand>();
    }
}