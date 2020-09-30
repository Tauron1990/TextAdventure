using System;
using Akka;
using Akka.Actor;
using Akka.Streams.Dsl;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Commands
{
    public sealed class RegisterForUpdate
    {
        public Sink<GameTime, NotUsed> UpdateInvoke { get; }

        public TimeSpan Interval { get; }

        public TimeSpan Next { get; }

        public Action<ICancelable>? RegisterCancel { get; }

        public RegisterForUpdate(Sink<GameTime, NotUsed> updateInvoke, TimeSpan interval, TimeSpan next, Action<ICancelable>? registerCancel = null)
        {
            UpdateInvoke = updateInvoke;
            Interval = interval;
            Next = next;
            RegisterCancel = registerCancel;
        }
    }
}