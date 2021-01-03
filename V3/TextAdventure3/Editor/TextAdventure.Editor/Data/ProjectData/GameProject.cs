using System;
using System.Collections.Immutable;

namespace TextAdventure.Editor.Data.ProjectData
{
    public sealed record GameProject(string GameName, Version GameVersion, ImmutableList<AssetEntry> Assets, ImmutableList<EntityEntry> Entries, ImmutableList<ScriptEntry> Scripts)
    {
        public static GameProject Create(string gameName, Version gameVersion) 
            => new(gameName, gameVersion, ImmutableList<AssetEntry>.Empty, ImmutableList<EntityEntry>.Empty, ImmutableList<ScriptEntry>.Empty);
    }
}