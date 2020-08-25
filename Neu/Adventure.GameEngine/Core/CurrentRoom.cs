using System;
using System.Reactive.Linq;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core
{
    [PublicAPI]
    public sealed class CurrentRoom : DynamicValue<IEntity>
    {
        public CurrentRoom(IObservableGroupManager collection) 
            : base(collection, new Group(typeof(Room), typeof(RoomData)))
        {
        }

        public RoomData Data => Value.GetComponent<RoomData>();

        public Room Name => Value.GetComponent<Room>();

        public EntityEvents Events => Value.GetComponent<EntityEvents>();

        public RoomCommands Commands => Value.GetComponent<RoomCommands>();

        protected override IEntity Transform(IEntity entity)
            => entity;

        protected override IObservable<bool> UpdateWhen(IEntity entity)
            => entity.GetComponent<RoomData>().IsPlayerIn.Where(v => v);
    }
}