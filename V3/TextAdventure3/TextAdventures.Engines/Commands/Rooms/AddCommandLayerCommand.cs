using System.Collections.Generic;
using JetBrains.Annotations;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands.Rooms
{
    public sealed class AddCommandLayerCommand : RoomCommand
    {
        public IEnumerable<CommandLayer> CommandLayers { get; }

        public AddCommandLayerCommand([NotNull] RoomId aggregateId, IEnumerable<CommandLayer> commandLayers) 
            : base(aggregateId) 
            => CommandLayers = commandLayers;

    }
}