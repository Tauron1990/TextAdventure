using Akkatecture.Core;
using JetBrains.Annotations;

namespace TextAdventures.Engine.Data
{
    public sealed class RoomId : Identity<RoomId>
    {
        public RoomId(string value)
            : base(value)
        {
        }
    }
}