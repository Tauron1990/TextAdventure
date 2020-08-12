using System;
using EcsRx.Components;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class GameInfo : IComponent
    {
        public int Version { get; }

        public DateTimeOffset SinceStart { get; }

        public string LastDisplay { get; set; }

        public GameInfo()
        {
        }

        [JsonConstructor]
        public GameInfo(int version, DateTimeOffset sinceStart, string lastDisplay)
        {
            Version = version;
            SinceStart = sinceStart;
            LastDisplay = lastDisplay;
        }
    }
}