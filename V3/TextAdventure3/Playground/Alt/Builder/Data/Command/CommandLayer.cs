using System;

namespace TextAdventures.Builder.Data.Command
{
    public sealed record CommandLayer(Name Name, bool Exclusive, Type Command, CommandMetadata Metadata);
}