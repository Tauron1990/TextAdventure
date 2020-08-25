using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Adventure.GameEngine.BuilderAlt
{
    [PublicAPI]
    public sealed class RoomConfiguration
    {
        private readonly IContentManagement _contentManagement;
        public IInternalGameConfiguration Config { get; }

        private readonly Dictionary<string, RoomBuilder> _rooms = new Dictionary<string, RoomBuilder>();
        
        internal RoomConfiguration(IInternalGameConfiguration config, IContentManagement contentManagement)
        {
            _contentManagement = contentManagement;
            Config = config;
        }
        
        internal IEnumerable<RoomBuilder> Rooms => _rooms.Values;

        public RoomBuilder FindRoom(string name)
            => _rooms[name];

        public RoomBuilder NewRoom(string name)
        {
            var builder = new RoomBuilder(name, this, s => _rooms.ContainsKey(s) ? _rooms[s] : null, Config, _contentManagement);
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