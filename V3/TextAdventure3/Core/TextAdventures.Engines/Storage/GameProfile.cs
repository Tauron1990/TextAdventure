using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TextAdventures.Engine.Storage
{
    [PublicAPI]
    public sealed record GameProfile(ImmutableDictionary<string, string> Saves, string Name, ImmutableDictionary<string, string> Meta, string RootPath)
    {
        public static readonly string SaveGameLocationBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tauron_Games");

        private const string ProfileExtension = ".profile";
        private const string SaveExtension = ".sav";

        public GameProfile GetSaveGame(string name)
        {
            if (Saves.ContainsKey(name))
                return this;

            return this with {Saves = Saves.Add(name, Path.Combine(RootPath, $"{Name}-Saves", name + SaveExtension))};
        }

        public static GameProfile Read(string path) => File.Exists(path)
            ? JsonConvert.DeserializeObject<GameProfile>(path)
            : new GameProfile(ImmutableDictionary<string, string>.Empty,
                Path.GetFileNameWithoutExtension(path),
                ImmutableDictionary<string, string>.Empty,
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
            foreach (var profile in Directory.EnumerateFiles(Path.Combine(SaveGameLocationBase, gameName), "*." + ProfileExtension))
            {
                GameProfile data;

                try
                {
                    data = JsonConvert.DeserializeObject<GameProfile>(File.ReadAllText(profile));
                }
                catch (Exception e)
                {
                    data = new GameProfile(ImmutableDictionary<string, string>.Empty, $"Error: {e.Message}", ImmutableDictionary<string, string>.Empty, "");
                }

                yield return data;
            }
        }
    }
}