using System;
using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Tauron.Akka;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class UpdateManagerActor : ExposedReceiveActor
    {
        private readonly ActorMaterializer _materializer;
        private readonly ICancelable _cancelable;
        private readonly Source<GameStartTime, NotUsed> _hub;

        public UpdateManagerActor()
        {
            GameStartTime startTime = new GameStartTime();
            _materializer = Context.Materializer(namePrefix:"UpdateManger");

            var producer = Source.Tick(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), startTime);

            var (cancelable, source) = producer.ToMaterialized(BroadcastHub.Sink<GameStartTime>(), Keep.Both).Run(_materializer);
            _cancelable = cancelable;
            _hub = source.Buffer(10, OverflowStrategy.DropBuffer);

            Receive<RegisterForUpdate>(u => u.Starter(_hub.Async()));
        }

        protected override void PostStop()
        {
            _cancelable.Cancel();
            _materializer.Dispose();

            base.PostStop();
        }
    }
}