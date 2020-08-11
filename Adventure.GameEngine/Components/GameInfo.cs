using System;
using EcsRx.Components;

namespace Adventure.GameEngine.Components
{
    public sealed class GameInfo : IComponent
    {
        public int Version { get; set; }

        public DateTimeOffset SinceStart { get; set; }

        public GameInfo()
        {
            
        }

        public GameInfo(int version, DateTimeOffset sinceStart)
        {
            Version = version;
            SinceStart = sinceStart;
        }
    }
}