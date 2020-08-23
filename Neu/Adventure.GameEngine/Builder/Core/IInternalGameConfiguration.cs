using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.ItemData;
using Adventure.GameEngine.Commands;

namespace Adventure.GameEngine.Builder.Core
{
    public interface IInternalGameConfiguration
    {
        CommandId RegisterCommand(Command command);

        Command GetCommand(CommandId id);

        ItemId RegisterItem(ItemBuilder item);
    }
}