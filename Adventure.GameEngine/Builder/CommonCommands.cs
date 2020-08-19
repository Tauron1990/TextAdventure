using System;
using System.Linq;
using Adventure.GameEngine.Blueprints;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Events;
using Adventure.GameEngine.Querys;
using Adventure.TextProcessing.Synonyms;
using Adventure.Utilities.Interfaces;
using CodeProject.ObjectPool.Specialized;
using EcsRx.Blueprints;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Infrastructure.Extensions;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class CommonCommands
    {
        private readonly IEventSystem _eventSystem;
        private readonly IContentManagement _management;
        private readonly IObservableGroup _rooms;
        private readonly IStringBuilderPool _stringBuilderPool;

        public CommonCommands(IEventSystem eventSystem, Game game)
        {
            _eventSystem = eventSystem;
            _management = game.Content;

            _stringBuilderPool = game.Container.Resolve<IStringBuilderPool>();
            _rooms = game.ObservableGroupManager.GetObservableGroup(new Group(typeof(Room), typeof(RoomData)));

            GoHandler = CreateGoHandler();
            LookHandler = CreateLookHandler();
            TakeHandler = CreateTakeHandler();
        }

        public (CommandHandler Handler, string Name) TakeHandler { get; }


        public (CommandHandler Handler, string Name) LookHandler { get; }


        public (CommandHandler Handler, string Name) GoHandler { get; }

        private (CommandHandler, string) CreateTakeHandler()
        {
            return (c =>
            {
                if (c.Verb.VerbCode != VerbCodes.Take) return null;

                var request = new TransferObjectToInventory(c.Noun.Replace, VerbCodes.Take);
                _eventSystem.Publish(request);
                return request.Result;
            }, "nimm");
        }

        private (CommandHandler, string) CreateLookHandler()
        {
            return (c =>
            {
                if (c.Verb.VerbCode != VerbCodes.Look) return null;

                var currentRoom = _rooms.Query(new QueryCurrentRoom()).Single();
                if (currentRoom == null)
                    return null;

                using var pooled = _stringBuilderPool.GetObject();
                var builder = pooled.StringBuilder;

                foreach (var interst in currentRoom.GetComponent<RoomData>().Pois.Where(interst => interst.Show))
                    builder.AppendLine(interst.Text.Format(_management));

                return builder.Length == 0 ? null : new LazyString(builder.ToString());
            }, "prüfen");
        }

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
                LookHandler,
                TakeHandler
            };

            return new RoomCommandSetup(
                (CommandHandler) Delegate.Combine(arr.Select(c => c.Handler).Cast<Delegate>().ToArray()),
                arr.Select(c => c.Name));
        }
    }
}