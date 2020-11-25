using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace TextAdventures.Engine.Internal
{
    public sealed class LoadingSequence
    {
        private LoadingSequence(TaskCompletionSource<IDisposable> compledtion, TimeSpan? timeout)
        {
            Compledtion = compledtion;
            Timeout     = timeout;
        }

        public TaskCompletionSource<IDisposable> Compledtion { get; }

        public TimeSpan? Timeout { get; }

        public static Task<IDisposable> Add(IActorRef loadingManager, TimeSpan? timeout = null)
        {
            var waiter = new TaskCompletionSource<IDisposable>();

            loadingManager.Tell(new LoadingSequence(waiter, timeout));

            return waiter.Task;
        }
    }
}