using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Adventure.GameEngine.Components;
using Adventure.TextProcessing;
using EcsRx.Events;
using EcsRx.Groups;
using EcsRx.Extensions;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class CommandParser : MultiEventReactionSystem
    {
        private readonly Parser _parser;

        private readonly BlockingCollection<string> _commands = new BlockingCollection<string>();

        public CommandParser(IEventSystem eventSystem, Parser parser) 
            : base(eventSystem)
        {
            _parser = parser;

            Task.Run(() =>
            {
                foreach (var com in _commands.GetConsumingEnumerable()) 
                    IncomingCommand(com);

                _commands.Dispose();
            });
        }

        public override IGroup Group { get; } = new Group(typeof(ReplayInfo));

        public void IncomingCommand(string eventData)
        {
            var gameTime = ObservableGroup.Where(e => e.HasComponent(typeof(GameInfo))).Select(e => e.GetComponent<GameInfo>()).Single();

            if (eventData != GameConsts.UpdateCommand)
            {
                foreach (var ent in ObservableGroup)
                    ent.GetComponent<ReplayInfo>().Add(eventData);

                EventSystem.Publish(_parser.ParseCommand(eventData));
            }

            EventSystem.Publish(gameTime.CreateGameTime());
        }

        protected override void Init() => Receive<string>(s => _commands.Add(s));
    }
}