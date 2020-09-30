using System;
using System.Collections.Generic;
using System.Threading;
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
        private sealed class InternalCancelable : ICancelable, IDisposable
        {
            private readonly ICancelable _cancelable;
            private readonly Action<ICancelable> _remover;
            private readonly CancellationTokenSource _tokenSource;

            public InternalCancelable(ICancelable cancelable, Action<ICancelable> remover)
            {
                _cancelable = cancelable;
                _remover = remover;
                _tokenSource = new CancellationTokenSource();
                _tokenSource.Token.Register(Cancel);
            }

            public void Cancel()
            {
                _remover(this);
                _cancelable.Cancel();
            }

            public void CancelAfter(TimeSpan delay) => _tokenSource.CancelAfter(delay);

            public void CancelAfter(int millisecondsDelay) => _tokenSource.CancelAfter(millisecondsDelay);

            public void Cancel(bool throwOnFirstException) => _tokenSource.Cancel(throwOnFirstException);

            public bool IsCancellationRequested => _cancelable.IsCancellationRequested;

            public CancellationToken Token => _tokenSource.Token;

            public void Dispose() => _tokenSource.Dispose();
        }

        private readonly ActorMaterializer _materializer;
        private readonly GameTime _gameTime = new GameTime();
        private readonly List<ICancelable> _updater = new List<ICancelable>();

        public UpdateManagerActor()
        {
            _materializer = Context.Materializer(namePrefix: "Updater");

            Receive<RegisterForUpdate>(u =>
            {
                var graph =
                    Source.Tick(u.Next, u.Interval, _gameTime)
                       .ToMaterialized(u.UpdateInvoke, (cancelable, _) => cancelable);

                var cancel = new InternalCancelable(graph.Run(_materializer), c => _updater.Remove(c));
                _updater.Add(cancel);

            });
        }

        protected override void PostStop()
        {
            _materializer.Dispose();
            foreach (var cancelable in _updater.ToArray()) 
                cancelable.Cancel();

            base.PostStop();
        }
    }
}