using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Commands;
using EcsRx.Collections;
using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class DelegateCommandProcessor : CommandProcessor<DelegateCommand>, ICommandProperties
    {
        private readonly Game _game;

        public DelegateCommandProcessor(IEventSystem eventSystem, Game game) : base(eventSystem)
            => _game = game;

        protected override void ProcessCommand(DelegateCommand command)
        {
            Command = command;
            
            Command.UpdatePropertys(this);
            Command.ActualHandler(this);
        }

        public DelegateCommand Command { get; private set; } = new DelegateCommand(new object(), p => {});

        public IObservableGroupManager ObservableGroupManager => _game.ObservableGroupManager;
    }
}