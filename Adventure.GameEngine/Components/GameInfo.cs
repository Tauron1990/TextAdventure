using System;
using EcsRx.Components;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class GameInfo : IComponent
    {
        public int Version { get; }

        public DateTimeOffset SinceStart { get; }

        public string LastDescription { get; set; }

        public string LastContent { get; set; }

        public GameInfo()
        {
        }

        [JsonConstructor]
        public GameInfo(int version, DateTimeOffset sinceStart, string lastDescription, string lastContent)
        {
            Version = version;
            SinceStart = sinceStart;
            LastDescription = lastDescription;
            LastContent = lastContent;
        }
    }
}