using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Blueprints;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
using Adventure.TextProcessing.Synonyms;
using Adventure.Utilities.Interfaces;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Groups.Observable;
using EcsRx.Extensions;
using EcsRx.Groups;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Rooms
{
    [PublicAPI]
    public sealed class CommonCommands
    {
        private readonly IEventSystem _eventSystem;
        private readonly IObservableGroup _rooms;

        public CommonCommands(IEventSystem eventSystem, Game game)
        {
            _eventSystem = eventSystem;
            _rooms = game.ObservableGroupManager.GetObservableGroup(new Group(typeof(Room), typeof(RoomData)));

            GoHandler = CreateGoHandler();
            LookHandler = CreateLookHandler();
        }

        public (CommandHandler Handler, string Name) LookHandler { get; }

        private (CommandHandler, string) CreateLookHandler()
        {
            return (c =>
            {
                var currwentRoom = _rooms.Query(new QuerryCurrentRoom()).Single();
            }, "prüfen");
        }

        public (CommandHandler Handler, string Name) GoHandler { get; }

        private (CommandHandler, string) CreateGoHandler()
        {
            return (c =>
            {
                if (c.Verb.VerbCode != VerbCodes.Go) return null;
                if (!Enum.TryParse<Direction>(c.Noun.Replace, out var direction)) return null;

                var evt = new GotoDirection(direction, c.Noun.Original);
                _eventSystem.Publish(evt);
                return evt.Result;
            }, "gehe");
        }

        public IBlueprint CreateCommonCommandBlueprint()
        {

            var arr = new[]
                      {
                          GoHandler,
                          LookHandler
                      };

            return new RoomCommandSetup(
                (CommandHandler) Delegate.Combine(arr.Select(c => c.Handler).Cast<Delegate>().ToArray()),
                arr.Select(c => c.Name));
        }

        private sealed class QuerryCurrentRoom : IObservableGroupQuery
        {
            public IEnumerable<IEntity> Execute(IObservableGroup observableGroup) => observableGroup.Where(e => e.GetComponent<RoomData>().IsPlayerIn.Value);
        }
        
    }
}