using Tauron.Application.Workshop.Mutating.Changes;

namespace TextAdventure.Editor.Operations.Events
{
    public record LoadFailedEvent(string Message) : MutatingChange;
}