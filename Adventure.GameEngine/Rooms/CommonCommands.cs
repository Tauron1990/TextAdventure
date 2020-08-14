using System;
using System.Linq;
using Adventure.GameEngine.Blueprints;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
using Adventure.TextProcessing;
using Adventure.TextProcessing.Interfaces;
using Adventure.TextProcessing.Synonyms;
using Adventure.Utilities;
using Adventure.Utilities.Interfaces;
using EcsRx.Blueprints;
using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Rooms
{
    [PublicAPI]
    public sealed class CommonCommands
    {
        private readonly IContentManagement _contentManagement;
        private readonly IEventSystem _eventSystem;

        public CommonCommands(IContentManagement contentManagement, IEventSystem eventSystem)
        {
            _contentManagement = contentManagement;
            _eventSystem = eventSystem;

            GoHandler = CreateGoHandler();
        }

        public (CommandHandler Handler, string Name) GoHandler { get; }

        private (CommandHandler Handler, string Name) CreateGoHandler()
        {
            return (c =>
            {
                if (c.Verb != VerbCodes.Go) return null;
                if (!Enum.TryParse<Direction>(c.Noun, out var direction)) return null;

                var evt = new GotoDirection(direction);
                _eventSystem.Publish(evt);
                return evt.Result;
            }, "gehe");
        }

        public IBlueprint CreateCommonCommandBlueprint()
        {

            var arr = new[]
                      {
                          GoHandler
                      };

            return new RoomCommandSetup(
                (CommandHandler) Delegate.Combine(arr.Select(c => c.Handler).Cast<Delegate>().ToArray()),
                arr.Select(c => c.Name));
        }
    }
}