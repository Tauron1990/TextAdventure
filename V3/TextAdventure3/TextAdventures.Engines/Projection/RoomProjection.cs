using System;
using System.Collections.Generic;
using ConcurrentCollections;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Projection.Base;

namespace TextAdventures.Engine.Projection
{
    public class RoomProjection : ProjectionBase<RoomId>
    {
        public Doorway[] Doorways
        {
            get => GetItem(Array.Empty<Doorway>());
            set => SetItem(value);
        }

        public ICollection<CommandLayer> CommandLayers => GetItem(() => new ConcurrentHashSet<CommandLayer>());
    }
}