using System;
using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Tauron.Akka;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class UpdateManagerActor : ExposedReceiveActor
    {
        private readonly ActorMaterializer _materializer;
        private readonly ICancelable _cancelable;
        private readonly Source<GameTime, NotUsed> _hub;

        public UpdateManagerActor()
        {
            GameTime time = new GameTime();
            _materializer = Context.Materializer(namePrefix:"UpdateManger");

            var producer = Source.Tick(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), time);

            var (cancelable, source) = producer.ToMaterialized(BroadcastHub.Sink<GameTime>(), Keep.Both).Run(_materializer);
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