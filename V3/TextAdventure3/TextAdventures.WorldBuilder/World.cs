using TextAdventures.Builder.Builder;
using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data;
using TextAdventures.Builder.Internal;

namespace TextAdventures.Builder
{
    public abstract class World
    {
        internal World()
        {
            
        }

        public static World Create(string saveDataPath)
            => new WorldImpl(saveDataPath);

        public abstract void Add(params INewAggregate[] aggregates);
        public abstract void Add(params INewProjector[] projectors);
        public abstract void Add(params INewQueryHandler[] handlers);
        public abstract void Add(params INewSaga[] sagas);

        public abstract RoomBuilder GetRoom(Name name);

        public abstract void AddActor(ActorBuilder builder);
    }
}