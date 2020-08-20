using System;
using System.Reactive.Linq;
using Adventure.GameEngine.Components;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core
{
    [PublicAPI]
    public sealed class PlayerInventory : DatabasedCollection<string>
    {
        public PlayerInventory(IObservableGroupManager collection)
            : base(collection, new Group(typeof(IngameObject)))
        {
        }

        protected override string Transform(IEntity entity)
        {
            return entity.GetComponent<IngameObject>().Id;
        }

        protected override IObservable<bool> AddWhen(IEntity entity)
        {
            return entity.GetComponent<IngameObject>()
                .Location.Select(l => l == GameConsts.PlayerInventoryId);
        }
    }
}