using System;
using Akkatecture.Core;
using JetBrains.Annotations;

namespace TextAdventures.Builder.Data.Actor
{
    [PublicAPI]
    public sealed class GameActorId : Identity<GameActorId>
    {
        public static Guid Namespace = new("8B014F42-CB5E-4F0C-9D75-47475A9CA4F4");

        public GameActorId(string value)
            : base(value) { }

        public static GameActorId FromName(Name name, bool isNpc)
        {
            string identifer = isNpc ? $"NPC-{name.Value}" : name.Value;
            return NewDeterministic(Namespace, identifer);
        }
    }
}