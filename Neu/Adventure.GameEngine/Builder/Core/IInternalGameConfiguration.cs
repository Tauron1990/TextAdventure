using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Commands;

namespace Adventure.GameEngine.Builder.Core
{
    public interface IInternalGameConfiguration
    {
        CommandId RegisterCommand(Command command);

        Command GetCommand(CommandId id);
    }
}