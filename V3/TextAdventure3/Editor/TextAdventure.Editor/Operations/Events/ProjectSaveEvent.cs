using Tauron.Application.Workshop.Mutating.Changes;

namespace TextAdventure.Editor.Operations.Events
{
    public record ProjectSaveEvent(bool IsOk, string Error) : MutatingChange;
}