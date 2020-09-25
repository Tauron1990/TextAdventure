using System;
using System.Collections.Concurrent;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data.Projection;
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

        protected override object Query(RoomQueryBase query)
        {
            return QueryResult.Error(new InvalidOperationException("Unkowen QueryType"));
        }
    }
}