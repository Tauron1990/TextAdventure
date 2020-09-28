using System;
using Akka;
using Akka.Streams.Dsl;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data;

namespace TextAdventures.Engine.Commands
{
    public sealed class RegisterForUpdate
    {
        public Action<Source<GameStartTime, NotUsed>> Starter { get; }

        public RegisterForUpdate(Action<Source<GameStartTime, NotUsed>> starter) => Starter = starter;
    }
}