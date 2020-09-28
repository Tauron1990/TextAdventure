using System.Collections.Generic;
using Akkatecture.Commands;

namespace TextAdventures.Builder.Data.Command
{
    public interface ICommandBuilder
    {
        IEnumerable<IGameCommand> Produce(CommandMetadata metadata);
    }
}