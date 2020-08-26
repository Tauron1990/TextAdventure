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
        private readonly IContentManagement _contentManagement;
        public GameConfiguration Config { get; }

        private readonly Dictionary<string, RoomBuilder> _rooms = new Dictionary<string, RoomBuilder>();
        
        internal RoomConfiguration(GameConfiguration config, IContentManagement contentManagement)
        {
            _contentManagement = contentManagement;
            Config = config;
        }
        
        internal IEnumerable<RoomBuilder> Rooms => _rooms.Values;

        public RoomBuilder FindRoom(string name)
            => _rooms[name];

        public RoomBuilder NewRoom(string name)
        {
            var builder = new RoomBuilder(name, Config, s => _rooms.ContainsKey(s) ? _rooms[s] : null, _contentManagement, this);
            _rooms.Add(name, builder);
            return builder;
        }

        internal void Validate()
        {
            foreach (var roomBuilder in _rooms.Select(p => p.Value).Cast<IEntityConfiguration>())
                roomBuilder.Validate();

            foreach (var prov in _rooms
                .Select(p => p.Value)
                .Cast<IWithSubEntity>()
                .SelectMany(r => r.SubEntities)
                .Cast<IEntityConfiguration>())
                prov.Validate();
        }
    }
}