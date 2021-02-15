using Tauron.Application.Workshop.StateManagement;

namespace TextAdventure.Editor.Operations.Command
{
    public sealed record TryLoadProjectCommand(string TargetPath, bool IsNew, string Name) : SimpleStateAction;
}