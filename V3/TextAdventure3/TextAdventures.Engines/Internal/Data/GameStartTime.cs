using System;
using Akkatecture.ValueObjects;

namespace TextAdventures.Engine.Internal.Data
{
    public sealed class GameStartTime : SingleValueObject<DateTime>
    {
        public GameStartTime(DateTime value) 
            : base(value) { }
    }
}