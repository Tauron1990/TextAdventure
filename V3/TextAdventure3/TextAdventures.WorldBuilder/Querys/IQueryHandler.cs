using Akka.Actor;

namespace TextAdventures.Builder.Querys
{
    public interface IQueryHandler
    {
        void Handle(IGameQuery query, IActorRef source);
    }
}