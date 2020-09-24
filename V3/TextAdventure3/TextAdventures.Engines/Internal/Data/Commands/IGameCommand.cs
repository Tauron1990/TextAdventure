using System;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public interface IGameCommand
    {
        Type Target { get; }
    }
}