using System;
using Akka.Actor;

namespace TextAdventures.Engine.Internal.Data
{
    public interface IAddAggregate
    {
        Type Target { get; }

        Props Props { get; }

        string Name { get; }
    }
}