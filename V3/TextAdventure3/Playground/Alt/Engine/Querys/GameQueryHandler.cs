using System;
using System.Collections.Concurrent;
using Akka.Actor;
using Akkatecture.Core;
using TextAdventures.Builder.Data.Querys;
using TextAdventures.Engine.Projection.Base;
using TextAdventures.Engine.Querys.Result;

namespace TextAdventures.Engine.Querys
{
    public abstract class GameQueryHandler<TQuery, TKey, TProjection> : IQueryHandler
        where TQuery : IGameQuery
        where TKey : IIdentity
        where TProjection : class, IProjectorData<TKey>, new()
    {
        protected GameQueryHandler(ConcurrentDictionary<TKey, TProjection> store)
            => Store = store;

        protected internal ConcurrentDictionary<TKey, TProjection> Store { get; }

        void IQueryHandler.Handle(IGameQuery query, IActorRef source)
        {
            try
            {
                var result = Query((TQuery) query);
                source.Tell(result, ActorCell.GetCurrentSelfOrNoSender());
            }
            catch (Exception e)
            {
                source.Tell(QueryResult.Error(e), ActorCell.GetCurrentSelfOrNoSender());
            }
        }

        protected abstract QueryResult Query(TQuery queryBase);
    }

    public abstract class GameQueryHandler<TQuery> : IQueryHandler
        where TQuery : IGameQuery
    {
        void IQueryHandler.Handle(IGameQuery query, IActorRef source)
        {
            try
            {
                var result = Query((TQuery) query);
                source.Tell(result, ActorCell.GetCurrentSelfOrNoSender());
            }
            catch (Exception e)
            {
                source.Tell(QueryResult.Error(e), ActorCell.GetCurrentSelfOrNoSender());
            }
        }

        protected abstract QueryResult Query(TQuery query);
    }
}