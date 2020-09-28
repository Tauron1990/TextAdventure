using System;
using Akka.Actor;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using TextAdventures.Builder.Commands;

namespace TextAdventures.Engine.Commands
{
    public sealed class NewAggregate<TManager, TAggregate, TId, TCommand> : INewAggregate
        where TManager : AggregateManager<TAggregate, TId, TCommand>, new()
        where TCommand : ICommand<TAggregate, TId>
        where TId : IIdentity 
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TId>
    {
        private object[] _params = Array.Empty<object>();

        Type INewAggregate.Target => typeof(TCommand);

        Props INewAggregate.Props => Props.Create<TManager>(_params);

        string INewAggregate.Name => typeof(TAggregate).Name;

        private NewAggregate()
        {
            
        }

        public NewAggregate<TManager, TAggregate, TId, TCommand> With(params object[] parameter)
        {
            _params = parameter;
            return this;
        }

        public static NewAggregate<TManager, TAggregate, TId, TCommand> Create()
            => new NewAggregate<TManager, TAggregate, TId, TCommand>();
    }
}