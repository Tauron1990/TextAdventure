using System;
using System.Collections.Concurrent;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Engine.Projection;
using TextAdventures.Engine.Querys;
using TextAdventures.Engine.Querys.Actors;
using TextAdventures.Engine.Querys.Result;

namespace TextAdventures.Engine.Internal.Querys
{
    public sealed class GameActorQueryHandler : GameQueryHandler<GameActorQueryBase, GameActorId, GameActorProjection>
    {
        public GameActorQueryHandler(ConcurrentDictionary<GameActorId, GameActorProjection> store)
            : base(store) { }

        protected override QueryResult Query(GameActorQueryBase queryBase) => QueryResult.Error(new InvalidOperationException("Unkowen QueryType"));
    }
}