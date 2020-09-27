using Akka.Actor;

namespace TextAdventures.Builder.Commands
{
    public interface INewSaga
    {
        Props SagaManager { get; }

        string Name { get; }
    }
}