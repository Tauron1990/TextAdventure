using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Tauron;

namespace TextAdventures.Engine.Storage
{
    public sealed record SaveInfo(string Name, string Path, string ProfileName);

    [PublicAPI]
    public sealed record GameProfile(ImmutableList<SaveInfo> Saves, string Name, ImmutableDictionary<string, string> Meta, string RootPath)
    {
        public static readonly string SaveGameLocationBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tauron_Games");

        private const string ProfileExtension = ".profile";
        private const string SaveExtension = ".sav";

        public GameProfile GetSaveGame(string name)
        {
            if (Saves.Any(i => i.Name == name))
                return this;

            return this with {Saves = Saves.Add(new SaveInfo(name, Path.Combine(RootPath, $"{Name}-Saves", name + SaveExtension), Name))};
        }

        public static GameProfile Read(string path) => File.Exists(path)
            ? JsonConvert.DeserializeObject<GameProfile>(path)
            : new GameProfile(ImmutableList<SaveInfo>.Empty, Path.GetFileNameWithoutExtension(path), ImmutableDictionary<string, string>.Empty,
                Path.GetDirectoryName(path) ?? throw new InvalidOperationException("No valid Profile Path"));

        public void Save()
            => File.WriteAllText(Path.Combine(RootPath, Name + ProfileExtension), JsonConvert.SerializeObject(this));

        public static GameProfile GetDefault(string gameName, string? name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "default";

            string fullPath = Path.Combine(SaveGameLocationBase, gameName, name + ProfileExtension);

            return Read(fullPath);
        }

        public static IEnumerable<GameProfile> GetProfiles(string gameName)
        {
            string path = Path.Combine(SaveGameLocationBase, gameName);
            path.CreateDirectoryIfNotExis();

            foreach (var profile in Directory.EnumerateFiles(path, "*." + ProfileExtension))
            {
                GameProfile data;

                try
                {
                    data = JsonConvert.DeserializeObject<GameProfile>(File.ReadAllText(profile));
                }
                catch (Exception e)
                {
                    data = new GameProfile(ImmutableList<SaveInfo>.Empty, $"Error: {e.Message}", ImmutableDictionary<string, string>.Empty, "");
                }

                yield return data;
            }
        }
    }
}