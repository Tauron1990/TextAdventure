using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Tauron;

namespace TextAdventures.Engine.Internal.Data
{
    public sealed class SaveProfile
    {
        public const string ProfileFile = "saves.info";

        public string Name { get; set; }

        public string DataPath { get; }

        public List<SaveInfo> Saves { get; } = new List<SaveInfo>();
        
        public int AutoSaveIncrement { get; set; }

        public int AutosaveCount { get; set; } = 5;

        public TimeSpan AutoSaveInterval { get; set; }

        public SaveProfile(string dataPath, string name)
        {
            DataPath = dataPath;
            Name = name;
            AutoSaveIncrement = 1;
            AutoSaveInterval = TimeSpan.FromMinutes(5);
        }

        [JsonConstructor]
        private SaveProfile(string name, string dataPath, int autoSaveIncrement, int autosaveCount, TimeSpan autoSaveInterval)
        {
            Name = name;
            DataPath = dataPath;
            AutoSaveIncrement = autoSaveIncrement;
            AutosaveCount = autosaveCount;
            AutoSaveInterval = autoSaveInterval;
        }

        public static IEnumerable<SaveProfile> GetProfiles(string root)
        {

            return root
                .EnumerateDirectorys()
                .Select(directory => Path.Combine(directory, ProfileFile))
                .Where(File.Exists)
                .Select(data =>
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<SaveProfile>(File.ReadAllText(data));
                    }
                    catch (Exception e)
                    {
                        if (e.IsCriticalApplicationException())
                            throw;
                        return null;
                    }
                })
                .Where(s => s != null)!;
        }

        public static SaveProfile Get(string dataPath)
        {
            dataPath.CreateDirectoryIfNotExis();
            var fullFile = Path.Combine(dataPath, ProfileFile);

            return File.Exists(fullFile) ? JsonConvert.DeserializeObject<SaveProfile>(File.ReadAllText(fullFile)) : new SaveProfile(dataPath);
        }

        public SaveInfo? GetSave(string? name) => Saves.Find(s => s.Name == name);

        internal void ClearSaves()
        {
            var files = new List<string> { "main.dat" };
            files.AddRange(Saves.Select(info => info.Name + ".dat"));

            Saves.Clear();

            foreach (var file in files)
            {
                Path.Combine(DataPath, file).DeleteFile();
            }
        }

        public SaveInfo NewSaveFile(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            if(name.Any(invalid.Contains))
                throw new ArgumentException("Invalid Savegame Name");
            if(GetSave(name) != null)
                throw new ArgumentException("Save game Exis", nameof(name));
            var file = new SaveInfo(name, Path.Combine(DataPath, $"{name}.dat"), DateTime.MinValue);
            Saves.Add(file);
            file.Profile = this;

            return file;
        }

        public string GetConnectionString() => GetConnectionString("main.dat");

        public string GetConnectionString(SaveInfo info) => GetConnectionString(info.Name + ".dat");

        private string GetConnectionString(string name) 
            => new SqliteConnectionStringBuilder { DataSource = name, Mode = SqliteOpenMode.ReadWriteCreate, Cache = SqliteCacheMode.Shared }.ConnectionString;

        public void Save() => File.WriteAllText(Path.Combine(DataPath, ProfileFile), JsonConvert.SerializeObject(this, Formatting.Indented));

        [OnDeserialized]
        private void OnDeserialization(StreamingContext sender)
        {
            foreach (var info in Saves) 
                info.Profile = this;
        }
    }
}