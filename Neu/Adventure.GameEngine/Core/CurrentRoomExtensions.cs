using System;
using System.Reactive.Linq;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;

namespace Adventure.GameEngine.Core
{
    public class RoomItems : DataBasedCollection<RoomItem>
    {
        private readonly CurrentRoom _currentRoom;

        public RoomItems(IObservableGroupManager manager, CurrentRoom currentRoom) 
            : base(manager, new Group(typeof(IngameObject)))
        {
            _currentRoom = currentRoom;
        }

        protected override RoomItem Transform(IEntity entity)
            => new RoomItem(entity, entity.GetComponent<IngameObject>());

        protected override IObservable<bool> AddWhen(IEntity entity)
        {
            var data = entity.GetComponent<IngameObject>();

            var tracker1 = _currentRoom.Select(_ => data);
            var tracker2 = data.Location.Select(_ => data);

            return tracker1.Concat(tracker2).Select(b => b.Location.Value == _currentRoom.Name.Name);
        }
    }
}