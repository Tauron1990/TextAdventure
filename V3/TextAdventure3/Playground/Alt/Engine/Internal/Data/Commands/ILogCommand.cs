using TextAdventures.Builder.Commands;
using TextAdventures.Builder.Data.Command;

namespace TextAdventures.Engine.Internal.Data.Commands
{
    public interface ILogCommand : IGameCommand, ICommandMetadata { }
}