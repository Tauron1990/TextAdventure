using System;
using System.Collections.Concurrent;
using System.Linq;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Projection;
using TextAdventures.Engine.Querys;
using TextAdventures.Engine.Querys.Result;
using TextAdventures.Engine.Querys.Room;

namespace TextAdventures.Engine.Internal.Querys
{
    public sealed class RoomQueryHandler : GameQueryHandler<RoomQueryBase, RoomId, RoomProjection>
    {
        public RoomQueryHandler(ConcurrentDictionary<RoomId, RoomProjection> store) 
            : base(store)
        {
        }

        protected override QueryResult Query(RoomQueryBase queryBase)
        {
            switch (queryBase)
            {
                case QueryCommandLayers commandQuery:
                    Store.TryGetValue(commandQuery.Id, out var proj);
                    return QueryResult.Compleded(proj?.CommandLayers.ToArray() ?? Array.Empty<CommandLayer>());
                default:
                    return QueryResult.Error(new InvalidOperationException("Unkowen QueryType"));
            }
        }
    }
}