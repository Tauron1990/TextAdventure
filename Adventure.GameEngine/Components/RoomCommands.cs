using System;
using System.Collections.Immutable;
using Adventure.TextProcessing.Interfaces;
using EcsRx.Components;

namespace Adventure.GameEngine.Components
{
    public sealed class RoomCommands : IComponent
    {
        public ImmutableList<Func<ICommand, string?>> Handler { get; private set; }

        public RoomCommands(Func<ICommand, string?> handler) => Handler = ImmutableList<Func<ICommand, string?>>.Empty.Add(handler);

        public RoomCommands()
        {
            Handler = ImmutableList<Func<ICommand, string?>>.Empty;
        }

        public void Add(Func<ICommand, string?> handler)
        {
            Handler = Handler.Add(handler);
        }
    }
}