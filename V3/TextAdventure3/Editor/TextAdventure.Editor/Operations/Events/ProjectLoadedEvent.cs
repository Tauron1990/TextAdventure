using Tauron.Application.Workshop.Mutating.Changes;
using TextAdventure.Editor.Data.ProjectData;

namespace TextAdventure.Editor.Operations.Events
{
    public record ProjectLoadedEvent(GameProject Project, string SourcePath) : MutatingChange;
}