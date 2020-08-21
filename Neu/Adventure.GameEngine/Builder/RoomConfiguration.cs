using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Builder.Core;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public sealed class RoomConfiguration
    {
        private readonly Dictionary<string, RoomBuilder> _rooms = new Dictionary<string, RoomBuilder>();

        public Dictionary<string, object> CustomData = new Dictionary<string, object>();

        internal RoomConfiguration(IInternalGameConfiguration config)
        {
        }
        
        internal IEnumerable<RoomBuilder> Rooms => _rooms.Values;

        public RoomBuilder NewRoom(string name)
        {
            var builder = new RoomBuilder(name, this, s => _rooms.ContainsKey(s) ? _rooms[s] : null);
            _rooms.Add(name, builder);
            return builder;
        }

        internal void Validate()
        {
            foreach (var (key, roomBuilder) in _rooms)
            {
                var result = roomBuilder.Validate();
                if (result != null)
                    throw new InvalidOperationException($"Room Validation Faild {key} Error {result}");
            }

            foreach (var prov in _rooms.SelectMany(r => r.Value.NewEntities))
                prov.Validate();
        }
    }
}