using Akka.Actor;

namespace TextAdventures.Engine.Querys
{
    public interface IQueryHandler
    {
        void Handle(IGameQuery query, IActorRef source);
    }
}