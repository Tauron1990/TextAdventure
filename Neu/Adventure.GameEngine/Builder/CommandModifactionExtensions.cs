using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public static class CommandModifactionExtensions
    {
        public static CommandModifaction<TReturn, LookCommand, RoomBuilder> WithRespond<TReturn>(this CommandModifaction<TReturn, LookCommand, RoomBuilder> source, LazyString respond)
        {
            source.Command.Responsd = respond;
            return source;
        }

        public static CommandModifaction<TReturn, TCommand, RoomBuilder> TriggersEvent<TReturn, TCommand>(this CommandModifaction<TReturn, TCommand, RoomBuilder> modifaction, string name, bool oneTime = true) 
            where TCommand : Command 
            where TReturn : IWithMetadata, IEntityConfiguration
        {
            modifaction.Command.TriggersEvent = new EventInfo(name, oneTime);
            return modifaction;
        }

        public static CommandModifaction<TReturn, TCommand, RoomBuilder> WithCategory<TReturn, TCommand>(this CommandModifaction<TReturn, TCommand, RoomBuilder> modifaction, string? category) 
            where TCommand : Command 
            where TReturn : IWithMetadata, IEntityConfiguration
        {
            modifaction.Command.Category = category;
            return modifaction;
        }

        public static CommandModifaction<TReturn, PickupCommand, RoomBuilder> WithRespond<TReturn>(this CommandModifaction<TReturn, PickupCommand, RoomBuilder> modifaction, LazyString description)
            where TReturn : IWithMetadata, IEntityConfiguration
        {
            modifaction.Command.Respond = description;
            return modifaction;
        }
    }
}