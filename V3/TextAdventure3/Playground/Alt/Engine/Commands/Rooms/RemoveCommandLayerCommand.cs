﻿using JetBrains.Annotations;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands.Rooms
{
    public sealed class RemoveCommandLayerCommand : RoomCommand
    {
        public RemoveCommandLayerCommand([NotNull] RoomId aggregateId, Name name)
            : base(aggregateId) => Name = name;

        public Name Name { get; }
    }
}