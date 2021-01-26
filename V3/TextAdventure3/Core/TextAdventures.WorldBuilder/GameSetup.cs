using System;
using System.Collections.Immutable;
using Tauron.Features;
using TextAdventures.Engine.Modules;

namespace TextAdventures.Builder
{
    public sealed record GameSetup(ImmutableList<GameObjectBlueprint> GameObjectBlueprints, ImmutableList<object> GameMmasterMessages,
        ImmutableDictionary<string, IPreparedFeature> GameProcesses, Action<Exception> Error, string SaveGame, ImmutableList<IModule> Modules);
}