using Akka.Actor;

namespace TextAdventures.Builder.Data.Commands
{
    public interface INewSaga
    {
        Props SagaManager { get; }

        string Name { get; }
    }
}