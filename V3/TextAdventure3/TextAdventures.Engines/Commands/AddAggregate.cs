using System;
using Akka.Actor;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using TextAdventures.Engine.Internal.Data;

namespace TextAdventures.Engine.Commands
{
    public sealed class AddAggregate<TManager, TAggregate, TId, TCommand> : IAddAggregate
        where TManager : AggregateManager<TAggregate, TId, TCommand> 
        where TCommand : ICommand<TAggregate, TId>
        where TId : IIdentity 
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TId>
    {
        Type IAddAggregate.Target => typeof(TCommand);

        Props IAddAggregate.Props => Props.Create<TManager>();

        string IAddAggregate.Name => typeof(TAggregate).Name;
    }
}