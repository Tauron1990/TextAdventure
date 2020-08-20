using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Components;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;

namespace Adventure.GameEngine.Querys
{
    public sealed class QueryNamedItem : IObservableGroupQuery
    {
        private readonly string _objectName;

        public QueryNamedItem(string objectName)
        {
            _objectName = objectName;
        }

        public IEnumerable<IEntity> Execute(IObservableGroup observableGroup)
        {
            return observableGroup.Where(e =>
            {
                var o = e.GetComponent<IngameObject>();

                return o.DisplayName == _objectName || o.Id == _objectName;
            });
        }
    }

    public sealed class QueryNamedItemFromRoom : IObservableGroupQuery
    {
        private readonly string _objectName;
        private readonly string _roomName;

        public QueryNamedItemFromRoom(string objectName, string roomName)
        {
            _objectName = objectName;
            _roomName = roomName;
        }

        public IEnumerable<IEntity> Execute(IObservableGroup observableGroup)
        {
            return observableGroup.Where(e =>
            {
                var o = e.GetComponent<IngameObject>();

                return o.Location.Value == _roomName && (o.DisplayName == _objectName || o.Id == _objectName);
            });
        }
    }
}