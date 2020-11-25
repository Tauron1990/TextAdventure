using System;
using System.Collections.Immutable;
using Akka.Actor;

namespace TextAdventures.Builder
{
    public sealed record GameSetup(ImmutableList<GameObjectBlueprint> GameObjectBlueprints,
        ImmutableDictionary<string, Props> GameProcesses, Action<Exception> Error);
}