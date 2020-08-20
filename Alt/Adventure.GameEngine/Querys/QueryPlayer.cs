using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Components;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Groups.Observable;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Querys
{
    [PublicAPI]
    public sealed class QueryPlayer : IObservableGroupQuery, IEntityCollectionQuery
    {
        private IEnumerable<IEntity> GetPlayer(IEnumerable<IEntity> entities)
        {
            yield return entities.Single(e =>
                e.HasComponent(typeof(Actor)) || e.HasComponent(typeof(IsPlayerControlled)));
        }
             
        public IEnumerable<IEntity> Execute(IObservableGroup observableGroup) => GetPlayer(observableGroup);
        public IEnumerable<IEntity> Execute(IEnumerable<IEntity> entityList) => GetPlayer(entityList);
    }
}