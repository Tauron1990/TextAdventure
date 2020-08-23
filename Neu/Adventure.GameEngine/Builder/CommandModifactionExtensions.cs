using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public static class CommandModifactionExtensions
    {
        public static CommandModifaction<TReturn, TCommand> TriggersEvent<TReturn, TCommand>(this CommandModifaction<TReturn, TCommand> modifaction, string name) 
            where TCommand : Command
        {
            modifaction.Command.TriggersEvent = name;
            return modifaction;
        }

        public static CommandModifaction<TReturn, TCommand> WithCategory<TReturn, TCommand>(this CommandModifaction<TReturn, TCommand> modifaction, string? category) 
            where TCommand : Command
        {
            modifaction.Command.Category = category;
            return modifaction;
        }

        public static CommandModifaction<TReturn, PickupCommand> WithRespond<TReturn>(this CommandModifaction<TReturn, PickupCommand> modifaction, LazyString description)
        {
            modifaction.Command.Respond = description;
            return modifaction;
        }
    }
}