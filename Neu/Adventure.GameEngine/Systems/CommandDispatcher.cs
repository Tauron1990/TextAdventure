using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Adventure.GameEngine.Systems.Events;
using EcsRx.Events;
using EcsRx.Plugins.ReactiveSystems.Custom;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [PublicAPI]
    public sealed class CommandDispatcher : EventReactionSystem<IncommingCommand>
    {
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly BlockingCollection<IncommingCommand> _toDispatch = new BlockingCollection<IncommingCommand>();

        public CommandDispatcher(IEventSystem eventSystem) 
            : base(eventSystem)
            => Task.Factory.StartNew(RunLoop, _cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

        public override void EventTriggered(IncommingCommand eventData)
            => _toDispatch.Add(eventData, _cancellation.Token);

        private void RunLoop()
        {
            try
            {
                foreach (var command in _toDispatch.GetConsumingEnumerable(_cancellation.Token))
                    command.Command.Dispatch(EventSystem);
            }
            catch (OperationCanceledException) { }
            finally
            {
                _cancellation.Dispose();
                _toDispatch.Dispose();
            }
        }
    }
}