using System.Linq;
using Akka.Actor;
using TextAdventures.Builder.Internal;
using TextAdventures.Engine.Commands.Actors;
using TextAdventures.Engine.Commands.Rooms;

namespace TextAdventures.Engine.Internal.WorldConstructor
{
    public sealed class WorldBuilder
    {
        private readonly WorldImpl _world;
        private readonly IActorRef _gameMaster;

        public WorldBuilder(WorldImpl world, IActorRef gameMaster)
        {
            _world = world;
            _gameMaster = gameMaster;
        }

        public void Load()
        {
            foreach (object message in _world.GameMasterMessages)
                _gameMaster.Tell(message);
        }

        public void Construct()
        {
            foreach (object message in _world.GameMasterMessages) 
                _gameMaster.Tell(message);

            foreach (var (id, room) in _world.Rooms) 
                _gameMaster.Tell(new CreateRoomCommand(id, room.Doorways.Values.ToArray()));

            foreach (var (id, actor) in _world.Actors) 
                _gameMaster.Tell(new CreateGameActorCommand(id, actor.PlayerType, actor.Name, actor.DisplayName, actor.Location));
        }
    }
}