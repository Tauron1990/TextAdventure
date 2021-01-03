using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Builder;
using TextAdventures.Engine.CommandSystem;
using TextAdventures.Engine.EventSystem;
using TextAdventures.Engine.Storage;

namespace TextAdventures.Engine
{
    [PublicAPI]
    public sealed class GameMaster
    {
        private readonly IActorRef _master;
        public ActorSystem System { get; }

        internal GameMaster(IActorRef master, ActorSystem system)
        {
            _master = master;
            System = system;
        }

        public void SendEvent(object evt)
            => _master.Tell(new GameEvent(evt));

        public void SendCommand(IGameCommand command)
            => _master.Tell(command);

        public void CreateObject(GameObjectBlueprint blueprint)
            => _master.Tell(blueprint);

        public void RegisterCommandProcessor(RegisterCommandProcessorBase registration)
            => _master.Tell(registration);

        public void MakeSaveGame(string name)
            => _master.Tell(new MakeSaveGame(name));
    }
}