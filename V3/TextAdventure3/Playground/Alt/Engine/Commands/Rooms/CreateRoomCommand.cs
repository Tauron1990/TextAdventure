﻿using System.Linq;
using TextAdventures.Builder.Builder;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Builder.Data.Rooms;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Commands.Rooms
{
    public sealed class CreateRoomCommand : RoomCommand
    {
        public CreateRoomCommand(Name roomName, Doorway[] doorways, Description detailDescription, Description description, CommandLayer[] commandLayers)
            : base(RoomId.FromName(roomName))
        {
            RoomName          = roomName;
            Doorways          = doorways;
            DetailDescription = detailDescription;
            Description       = description;
            CommandLayers     = commandLayers;
        }

        public Name RoomName { get; }

        public Doorway[] Doorways { get; }

        public CommandLayer[] CommandLayers { get; }

        public Description Description { get; }

        public Description DetailDescription { get; }

        public static CreateRoomCommand CreateFrom(RoomBuilder builder) =>
            new(builder.Name, builder.Doorways.Values.ToArray(), builder.DetailDescription, builder.Description,
                builder.CommandLayers.ToArray());
    }
}