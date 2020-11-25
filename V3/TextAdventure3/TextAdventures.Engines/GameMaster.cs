using Akka.Actor;
using JetBrains.Annotations;

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
    }
}