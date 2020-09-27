﻿using System.Collections.Concurrent;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Engine.Projection
{
    public sealed class GameProjection
    {
        public ConcurrentDictionary<RoomId, RoomProjection> Rooms { get; } = new ConcurrentDictionary<RoomId, RoomProjection>();

        public ConcurrentDictionary<GameActorId, GameActorProjection> GameActors { get; } = new ConcurrentDictionary<GameActorId, GameActorProjection>();
    }
}