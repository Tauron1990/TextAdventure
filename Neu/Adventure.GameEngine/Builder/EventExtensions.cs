using System;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.Events;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public static class EventExtensions
    {
        public static TReturn ReactiToEvent<TReturn, TData>(this TReturn eventable, string eventName, Action<EventConfiguration<TReturn, TData>> config) 
            where TReturn : IWithMetadata, IEntityConfiguration, IEventable<TReturn, TData>
        {
            var configInfo = new EventConfiguration<TReturn, TData>(eventable, eventable.EventData);
            config(configInfo);
            configInfo.Register(eventable, eventName);

            return eventable;
        }

        public static void ModifyObject<TReturn, TObject>(this EventConfiguration<TReturn, TObject> config, Action<TObject> changer) 
            where TReturn : IWithMetadata, IEntityConfiguration
            => config.Registrar = new ObjectModificationRegistrar<TReturn, TObject>(changer);

        public static void ChangeItemDescription<TReturn>(this EventConfiguration<TReturn, ItemBluePrint> config, LazyString description)
            where TReturn : IWithMetadata, IEntityConfiguration =>
            config.Registrar = new ChnageItemDescription<TReturn>(description);

        public static void SendCommand<TSource, TData>(this EventConfiguration<TSource, TData> config)
            where TData : Command
            where TSource : IWithMetadata, IEntityConfiguration =>
            config.Registrar = new SendDataRegistrar<TSource, TData>();
    }
}