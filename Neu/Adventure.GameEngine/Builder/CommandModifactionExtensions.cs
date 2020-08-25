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
        public static CommandModifaction<TReturn, LookCommand, TEventSource> WithRespond<TReturn, TEventSource>(this CommandModifaction<TReturn, LookCommand, TEventSource> source, LazyString respond)
            where TEventSource : IWithMetadata, IEntityConfiguration
        {
            source.Command.Responsd = respond;
            return source;
        }

        public static CommandModifaction<TReturn, TCommand, TEventSource> TriggersEvent<TReturn, TCommand, TEventSource>
            (this CommandModifaction<TReturn, TCommand, TEventSource> modifaction, string name) 
            where TCommand : Command 
            where TReturn : IWithMetadata, IEntityConfiguration
            where TEventSource : IWithMetadata, IEntityConfiguration
        {
            modifaction.Command.TriggersEvent = name;
            return modifaction;
        }

        public static CommandModifaction<TReturn, TCommand, TEventSource> WithCategory<TReturn, TCommand, TEventSource>
            (this CommandModifaction<TReturn, TCommand, TEventSource> modifaction, string? category) 
            where TCommand : Command 
            where TReturn : IWithMetadata, IEntityConfiguration
            where TEventSource : IWithMetadata, IEntityConfiguration
        {
            modifaction.Command.Category = category;
            return modifaction;
        }

        public static CommandModifaction<TReturn, PickupCommand, TEventSource> WithRespond<TReturn, TEventSource>
            (this CommandModifaction<TReturn, PickupCommand, TEventSource> modifaction, LazyString description)
            where TReturn : IWithMetadata, IEntityConfiguration
            where TEventSource : IWithMetadata, IEntityConfiguration
        {
            modifaction.Command.Respond = description;
            return modifaction;
        }
    }
}