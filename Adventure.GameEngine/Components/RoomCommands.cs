﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Adventure.TextProcessing.Interfaces;
using EcsRx.Components;

namespace Adventure.GameEngine.Components
{
    public delegate string? CommandHandler(ICommand command);


    public sealed class RoomCommands : IComponent, INotSerialized
    {
        public ImmutableList<CommandHandler> Handler { get; private set; }

        public HashSet<string> ProcessedCommands { get; } = new HashSet<string>();

        public RoomCommands() 
            => Handler = ImmutableList<CommandHandler>.Empty;

        public void Add(CommandHandler handler, IEnumerable<string> processedCommands)
        {
            foreach (var command in processedCommands) 
                ProcessedCommands.Add(command);
            Handler = Handler.Add(handler);
        }
    }
}