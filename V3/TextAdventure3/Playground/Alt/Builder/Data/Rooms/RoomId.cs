using System;
using Akkatecture.Core;
using JetBrains.Annotations;

namespace TextAdventures.Builder.Data.Rooms
{
    [PublicAPI]
    public sealed class RoomId : Identity<RoomId>
    {
        public static readonly Guid RoomNamespace = new("497C671A-F2F8-4BCE-BF84-5C4692B8DFBD");

        public RoomId(string value)
            : base(value) { }

        public static RoomId FromName(Name name)
            => NewDeterministic(RoomNamespace, name.Value);
    }
}