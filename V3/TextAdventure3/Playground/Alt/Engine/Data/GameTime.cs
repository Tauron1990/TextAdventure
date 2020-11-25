using System;

namespace TextAdventures.Engine.Data
{
    public sealed class GameTime
    {
        public DateTime SessionStartTime { get; } = DateTime.UtcNow;
    }
}