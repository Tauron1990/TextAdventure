using System;
using System.Diagnostics;
using EcsRx.Components;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class GameInfo : IComponent
    {
        private readonly DateTimeOffset _creationTime = DateTimeOffset.UtcNow;
        private readonly Stopwatch _updateInterval = Stopwatch.StartNew();

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

        public GameTime CreateGameTime()
        {
            var time = _updateInterval.Elapsed;
            _updateInterval.Restart();

            return new GameTime(_creationTime, time);
        }
    }
}