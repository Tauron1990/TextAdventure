using System;
using System.Diagnostics;

namespace TextAdventures.Engine.Data
{
    public sealed class GameTime
    {
        public DateTime StartTime { get; } = DateTime.UtcNow;

        public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
    }
}