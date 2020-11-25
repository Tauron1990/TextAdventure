using System;
using Akka.Actor;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using JetBrains.Annotations;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data.Commands;

namespace TextAdventures.Builder.Data
{
    [PublicAPI]
    public abstract class ComponentBlueprint<TIdentidy, TAggregate, TCommand> : IComponentBlueprint
        where TAggregate : ReceivePersistentActor, IGameAggregate, IAggregateRoot<TIdentidy> 
        where TIdentidy : IIdentity
        where TCommand : ICommand<TAggregate, TIdentidy>, IGameCommand
    {
        public class BasicGameAggergateManager : AggregateManager<TAggregate, TIdentidy, TCommand>
        {
            
        }

        Type INewAggregate.Target => typeof(TCommand);

        Props INewAggregate.Props => Props.Create<TAggregate>();

        string INewAggregate.Name => typeof(TAggregate).Name;
        
        protected Props CreateProps() => Props.Create<BasicGameAggergateManager>();
    }
}