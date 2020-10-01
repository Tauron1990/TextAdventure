using Akka.Actor;
using TextAdventures.Builder.Builder;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Internal;

namespace TextAdventures.Builder
{
    public abstract class World
    {
        public static World Create(string saveDataPath, string profileName)
            => new WorldImpl(saveDataPath, profileName);

        public abstract void Add(Props props, string name);
        public abstract void Add(params INewAggregate[] aggregates);
        public abstract void Add(params INewProjector[] projectors);
        public abstract void Add(params INewQueryHandler[] handlers);
        public abstract void Add(params INewSaga[] sagas);

        public abstract RoomBuilder GetRoom(Name name);

        public abstract void AddActor(ActorBuilder builder);
    }
}