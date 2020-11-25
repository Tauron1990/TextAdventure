using System.Collections.Generic;
using TextAdventures.Builder.Commands;

namespace TextAdventures.Builder.Data.Command
{
    public interface ICommandBuilder
    {
        IEnumerable<IGameCommand> Produce(CommandMetadata metadata);
    }
}