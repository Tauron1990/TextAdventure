using System;
using Newtonsoft.Json;

namespace TextAdventures.Engine.Internal.Data
{
    public sealed class SaveInfo
    {
        public string Name { get; set; }

        public string Path { get; }

        public DateTime SaveTime { get; set; }

        [JsonConstructor]
        internal SaveInfo(string name, string path, DateTime saveTime)
        {
            Name = name;
            Path = path;
            SaveTime = saveTime;
        }
    }
}