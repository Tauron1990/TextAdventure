using System;
using System.Linq;
using System.Reactive.Linq;
using Adventure.GameEngine.Systems.Components;
using Adventure.GameEngine.Systems.Events;
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
        private readonly Lazy<GameInfo> _gameInfo;

        public override IGroup Group { get; } = new Group(typeof(Room), typeof(RoomData));

        public UIUpdater(IEventSystem eventSystem, Game game)
            : base(eventSystem)
        {
            _game = game;
            _gameInfo = new Lazy<GameInfo>(() => _game.ObservableGroupManager.GetObservableGroup(new Group(typeof(GameInfo))).Single().GetComponent<GameInfo>());
        }

        protected override void Init()
        {
            Receive<QueryLastEntry>(c =>
            {
                var data = _gameInfo.Value;
                EventSystem.Publish(new UpdateTextContent(data.LastContent, data.LastDescription));
            });
            Receive<CommandContentUpdate>(CommandExecuted);
        }

        private void CommandExecuted(CommandContentUpdate result) 
            => SendUpdate(result.Result?.Format(_game.Content), null);

        public IObservable<IEntity> ReactToEntity(IEntity entity)
        {
            var data = entity.GetComponent<RoomData>();

            var tracker1 = data.IsPlayerIn.Where(b => b).Select(b => entity);
            var tracker2 = data.Description.Where(s => data.IsPlayerIn.Value).Select(s => entity);

            return tracker1.Concat(tracker2);
        }

        public void Process(IEntity entity) 
            => SendUpdate(null, GetText(entity.GetComponent<RoomData>().Description.Value));

        private void SendUpdate(string? content, string? description)
        {
            var data = _gameInfo.Value;
            if (content != null)
                data.LastContent = content;
            if (description != null)
                data.LastDescription = description;

            EventSystem.Publish(new UpdateTextContent(content, description));
        }

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