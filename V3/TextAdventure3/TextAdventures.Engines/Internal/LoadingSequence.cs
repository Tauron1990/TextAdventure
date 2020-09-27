using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace TextAdventures.Engine.Internal
{
    public sealed class LoadingSequence
    {
        public TaskCompletionSource<IDisposable> Compledtion { get; }

        public TimeSpan? Timeout { get; }

        private LoadingSequence(TaskCompletionSource<IDisposable> compledtion, TimeSpan? timeout)
        {
            Compledtion = compledtion;
            Timeout = timeout;
        }

        public static Task<IDisposable> Add(IActorRef loadingManager, TimeSpan? timeout = null)
        {
            var waiter = new TaskCompletionSource<IDisposable>();

            loadingManager.Tell(new LoadingSequence(waiter, timeout));

            return waiter.Task;
        }
    }
}