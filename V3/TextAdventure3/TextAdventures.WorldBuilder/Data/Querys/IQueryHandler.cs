using Akka.Actor;

namespace TextAdventures.Builder.Data.Querys
{
    public interface IQueryHandler
    {
        void Handle(IGameQuery query, IActorRef source);
    }
}