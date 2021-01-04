using Tauron.Application.Workshop.StateManagement;

namespace TextAdventure.Editor.Operations.Command
{
    public sealed record ChangeGameNameCommand(string Name, string Version) : SimpleStateAction, ISaveRelevant;
}