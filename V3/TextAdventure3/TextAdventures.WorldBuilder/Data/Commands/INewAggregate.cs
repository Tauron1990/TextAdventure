using System;
using Akka.Actor;

namespace TextAdventures.Builder.Data.Commands
{
    public interface INewAggregate
    {
        Type Target { get; }

        Props Props { get; }

        string Name { get; }
    }
}