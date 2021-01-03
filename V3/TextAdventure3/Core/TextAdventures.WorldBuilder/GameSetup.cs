using System;
using System.Collections.Immutable;
using Akka.Actor;
using TextAdventures.Engine.Modules;

namespace TextAdventures.Builder
{
    public sealed record GameSetup(ImmutableList<GameObjectBlueprint> GameObjectBlueprints, ImmutableList<object> GameMmasterMessages,
        ImmutableDictionary<string, Props> GameProcesses, Action<Exception> Error, string SaveGame, ImmutableList<IModule> Modules);
}