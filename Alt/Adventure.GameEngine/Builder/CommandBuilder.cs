using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Blueprints;
using Adventure.GameEngine.Components;
using Adventure.TextProcessing;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class CommandBuilder
    {
        private readonly RoomBuilder _builder;

        private readonly List<(string Name, CommandHandler Handler)> _handlers =
            new List<(string Name, CommandHandler Handler)>();

        private readonly Parser _parser;

        public CommandBuilder(RoomBuilder builder, Parser parser)
        {
            _builder = builder;
            _parser = parser;
        }

        public CommandBuilder WithCommand(string name, CommandHandler handler)
        {
            _handlers.Add((name, handler));
            return this;
        }

        public RoomBuilder AndRegister()
        {
            _builder.Blueprints.Add(new RoomCommandSetup(
                (CommandHandler) Delegate.Combine(_handlers.Select(e => e.Handler).Cast<Delegate>().ToArray()),
                _handlers.Select(e => e.Name)));
            return _builder;
        }
    }
}