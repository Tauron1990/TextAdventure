using System;
using System.Collections.Generic;
using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data.Command
{
    public sealed class CommandLayer
    {
        public Name Name { get; }

        public bool Exclusive { get; }

        public Type Command { get; }

        public CommandMetadata Metadata { get; set; }

        public CommandLayer(Name name, bool exclusive, Type command, CommandMetadata metadata)
        {
            Name = name;
            Exclusive = exclusive;
            Command = command;
            Metadata = metadata;
        }
    }
}