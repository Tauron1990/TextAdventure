using System;
using System.Collections.Concurrent;
using Akka.Actor;
using Akkatecture.Core;
using TextAdventures.Engine.Internal.Data.Projection;
using TextAdventures.Engine.Querys.Result;

namespace TextAdventures.Engine.Querys
{
    public abstract class GameQueryHandler<TQuery, TKey, TProjection> : IQueryHandler
        where TQuery : IGameQuery
        where TKey : IIdentity
        where TProjection : class, IProjectorData<TKey>, new()
    {
        protected internal ConcurrentDictionary<TKey, TProjection> Store { get; }

        protected GameQueryHandler(ConcurrentDictionary<TKey, TProjection> store) 
            => Store = store;

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

        protected abstract object Query(TQuery query);
    }

    public abstract class GameQueryHandler<TQuery> : IQueryHandler
        where TQuery : IGameQuery
    {
        protected GameQueryHandler()
        {}

        void IQueryHandler.Handle(IGameQuery query, IActorRef source)
        {
            try
            {
                var result = Query((TQuery)query);
                source.Tell(result, ActorCell.GetCurrentSelfOrNoSender());
            }
            catch (Exception e)
            {
                source.Tell(QueryResult.Error(e), ActorCell.GetCurrentSelfOrNoSender());
            }
        }

        protected abstract object Query(TQuery query);
    }
}