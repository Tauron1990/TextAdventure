using System.Collections.Generic;
using System.Linq;
using TextAdventures.Builder.Builder;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Builder.Internal
{
    public sealed class WorldImpl : World
    {
        public string DataPath { get; }

        public Dictionary<Name, RoomBuilder> Rooms { get; } = new Dictionary<Name, RoomBuilder>();

        public Dictionary<GameActorId, ActorBuilder> Actors { get; } = new Dictionary<GameActorId, ActorBuilder>();

        public List<object> GameMasterMessages { get; } = new List<object>();

        internal WorldImpl(string dataPath) => DataPath = dataPath;

        public override void Add(params INewAggregate[] aggregates) => GameMasterMessages.AddRange(aggregates);

        public override void Add(params INewProjector[] projectors) => GameMasterMessages.AddRange(projectors);

        public override void Add(params INewQueryHandler[] handlers) => GameMasterMessages.AddRange(handlers);

        public override void Add(params INewSaga[] sagas) => GameMasterMessages.AddRange(sagas);

        public override RoomBuilder GetRoom(Name name)
        {
            if (Rooms.TryGetValue(name, out var room))
                return room;
            
            room = new RoomBuilder(this, RoomId.FromName(name), name);
            Rooms.Add(name, room);
            return room;
        }

        public override void AddActor(ActorBuilder builder) 
            => Actors.Add(GameActorId.FromName(builder.Name, builder.PlayerType.Value != Player.Player), builder);

        internal RoomBuilder FindById(RoomId id)
            => Rooms.First(b => b.Value.Self == id).Value;
    }
}