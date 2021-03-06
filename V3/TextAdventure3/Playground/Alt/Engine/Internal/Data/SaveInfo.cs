﻿using System;
using Newtonsoft.Json;

namespace TextAdventures.Engine.Internal.Data
{
    public sealed class SaveInfo
    {
        [JsonConstructor]
        internal SaveInfo(string name, string path, DateTime saveTime)
        {
            Name     = name;
            Path     = path;
            SaveTime = saveTime;
        }

        public string Name { get; set; }

        public string Path { get; }

        public DateTime SaveTime { get; set; }

        [JsonIgnore]
        public SaveProfile Profile { get; internal set; }
    }
}