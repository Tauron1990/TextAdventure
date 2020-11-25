using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Akka.Actor;
using TextAdventures.Builder.Builder;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Builder.Data.Commands;
using TextAdventures.Builder.Data.Rooms;

namespace TextAdventures.Builder.Internal
{
    public sealed class WorldImpl : World
    {
        public WorldImpl(string dataPath, string profileName)
        {
            DataPath    = dataPath;
            ProfileName = profileName;
        }

        public string DataPath    { get; }
        public string ProfileName { get; }

        public ImmutableList<object> GameMasterMessages { get; private set; } = ImmutableList<object>.Empty;

        public ImmutableDictionary<string, Props> ActorProps { get; private set; } = ImmutableDictionary<string, Props>.Empty;

        public ImmutableList<GameObjectBlueprint> GameObjects { get; private set; } = ImmutableList<GameObjectBlueprint>.Empty;

        public override void Add(Props props, string name) => ActorProps = ActorProps.Add(name, props);

        public override void Add(params INewAggregate[] aggregates) => GameMasterMessages = GameMasterMessages.AddRange(aggregates);

        public override void Add(params INewProjector[] projectors) => GameMasterMessages = GameMasterMessages.AddRange(projectors);

        public override void Add(params INewQueryHandler[] handlers) => GameMasterMessages = GameMasterMessages.AddRange(handlers);

        public override void Add(params            INewSaga[]            sagas)   => GameMasterMessages = GameMasterMessages.AddRange(sagas);
        public override void AddGameObjects(params GameObjectBlueprint[] objects)
        {
            
        }
    }
}