using System;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class GameTime
    {
        public DateTimeOffset SinceGameStart { get; }

        public TimeSpan SinceLastUpdate { get; }

        public GameTime(DateTimeOffset sinceGameStart, TimeSpan sinceLastUpdate)
        {
            SinceGameStart = sinceGameStart;
            SinceLastUpdate = sinceLastUpdate;
        }
    }
}