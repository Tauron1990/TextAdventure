using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core.Querys
{
    [PublicAPI]
    public sealed class QueryNamedItem : IObservableGroupQuery, IEntityCollectionQuery
    {
        private readonly string _objectName;

        public QueryNamedItem(string objectName)
        {
            _objectName = objectName;
        }

        public IEnumerable<IEntity> Execute(IObservableGroup observableGroup)
            => Execute((IEnumerable<IEntity>) observableGroup);

        public IEnumerable<IEntity> Execute(IEnumerable<IEntity> entityList)
        {
            return entityList.Where(e =>
            {
                var o = e.GetComponent<IngameObject>();

                return o.DisplayName == _objectName || o.Id == _objectName;
            });
        }
    }

    [PublicAPI]
    public sealed class QueryNamedItemFromRoom : IObservableGroupQuery, IEntityCollectionQuery
    {
        private readonly string _objectName;
        private readonly string _roomName;

        public QueryNamedItemFromRoom(string objectName, string roomName)
        {
            _objectName = objectName;
            _roomName = roomName;
        }

        public IEnumerable<IEntity> Execute(IObservableGroup observableGroup)
            => Execute((IEnumerable<IEntity>) observableGroup);

        public IEnumerable<IEntity> Execute(IEnumerable<IEntity> entityList)
        {
            return entityList.Where(e =>
            {
                var o = e.GetComponent<IngameObject>();

                return o.Location.Value == _roomName && (o.DisplayName == _objectName || o.Id == _objectName);
            });
        }
    }
}