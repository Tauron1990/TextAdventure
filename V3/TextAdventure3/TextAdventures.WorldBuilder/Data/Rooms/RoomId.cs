using System;
using Akkatecture.Core;

namespace TextAdventures.Builder.Data.Rooms
{
    public sealed class RoomId : Identity<RoomId>
    {
        public static readonly Guid RoomNamespace = new Guid("497C671A-F2F8-4BCE-BF84-5C4692B8DFBD");

        public RoomId(string value)
            : base(value)
        { }

        public static RoomId FromName(Name name)
            => NewDeterministic(RoomNamespace, name.Value);
    }
}