using System.Collections.Concurrent;
using System.Threading.Tasks;
using Adventure.GameEngine.Components;
using Adventure.TextProcessing;
using Adventure.TextProcessing.Synonyms;
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

        public CommandParser(IEventSystem eventSystem) 
            : base(eventSystem)
        {
            _parser = new Parser(new VerbSynonyms(), new NounSynonyms(), new PrepositionMapping());

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
            foreach (var ent in ObservableGroup) 
                ent.GetComponent<ReplayInfo>().Add(eventData);
            
            if(eventData != GameConsts.UpdateCommand)
                EventSystem.Publish(_parser.ParseCommand(eventData));

            //TODO Generic Update for All
        }

        protected override void Init() => Receive<string>(s => _commands.Add(s));
    }
}