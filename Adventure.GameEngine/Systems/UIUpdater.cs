using System;
using System.Reactive.Linq;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [PublicAPI]
    public sealed class UIUpdater : MultiEventReactionSystem, IReactToEntitySystem
    {
        private readonly Game _game;
        public override IGroup Group { get; } = new Group(typeof(Room), typeof(RoomData));

        public UIUpdater(IEventSystem eventSystem, Game game)
            : base(eventSystem) => _game = game;

        protected override void Init() 
            => Receive<CommandExecutionCompled>(CommandExecuted);

        private void CommandExecuted(CommandExecutionCompled result) 
            => EventSystem.Publish(new UpdateTextContent(GetText(result.Result), null));

        public IObservable<IEntity> ReactToEntity(IEntity entity)
        {
            var data = entity.GetComponent<RoomData>();

            var tracker1 = data.IsPlayerIn.Where(b => b).Select(b => entity);
            var tracker2 = data.Description.Where(s => data.IsPlayerIn.Value).Select(s => entity);

            return tracker1.Concat(tracker2);
        }

        public void Process(IEntity entity) 
            => EventSystem.Publish(new UpdateTextContent(null, GetText(entity.GetComponent<RoomData>().Description.Value)));

        private string? GetText(string? data)
        {
            if (data == null)
                return null;

            if (_game.Content.Exists(data))
                return _game.Content.RetrieveContentItem<string?>(data, null) ?? data;
            return data;
        }
    }
}