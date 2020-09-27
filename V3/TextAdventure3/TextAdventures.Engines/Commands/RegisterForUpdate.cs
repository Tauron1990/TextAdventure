using System;
using Akka;
using Akka.Streams.Dsl;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Commands
{
    public sealed class RegisterForUpdate
    {
        public Action<Source<GameTime, NotUsed>> Starter { get; }

        public RegisterForUpdate(Action<Source<GameTime, NotUsed>> starter) => Starter = starter;
    }
}