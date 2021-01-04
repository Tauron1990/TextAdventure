using Tauron.Application.Workshop.Mutating.Changes;
using TextAdventure.Editor.Data.ProjectData;

namespace TextAdventure.Editor.Operations.Events
{
    public record ProjectSaveEvent(bool IsOk, string Error) : MutatingChange;
    public record ProjectLoadedEvent(GameProject Project, string SourcePath) : MutatingChange;
    public record LoadFailedEvent(string Message) : MutatingChange;
}