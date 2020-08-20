using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Components;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;

namespace Adventure.GameEngine.Querys
{
    public sealed class QueryCurrentRoom : IObservableGroupQuery, IEntityCollectionQuery
    {
        private static IEnumerable<IEntity> GetRoom(IEnumerable<IEntity> entities)
        {
            yield return entities.Single(e => e.GetComponent<RoomData>().IsPlayerIn.Value);
        }

        public IEnumerable<IEntity> Execute(IObservableGroup observableGroup) => GetRoom(observableGroup);
        public IEnumerable<IEntity> Execute(IEnumerable<IEntity> entityList) => GetRoom(entityList);
    }
}