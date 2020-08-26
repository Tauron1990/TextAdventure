using System;
using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.Events;
using Adventure.GameEngine.Builder.ItemData;
using Adventure.GameEngine.Builder.Poi;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using Adventure.GameEngine.Systems.Components;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public static class ItemExtensions
    {
        public static TReturn WithLook<TReturn>(this IRoomItemBuilder<TReturn> source, LazyString? respond = null)  
            where TReturn : IHasRoot
        {
            source.Impl.Root.AddBlueprint(
                new RoomCommand(
                    new LookCommand(source.Id)
                    {
                        Responsd = respond,
                        Category = GameConsts.LookCategory
                    },
                    LazyString.New(GameConsts.LookAtCommand).AddParameters(source.Id)));
            return source.Impl;
        }

        public static TReturn WithLook<TReturn>(this IRoomItemBuilder<TReturn> source, Action<CommandModifaction<TReturn, LookCommand, RoomBuilder>> commandConfig)
            where TReturn : IHasRoot, IEventable<RoomBuilder, ItemBluePrint>
        {
            var command = new LookCommand(source.Id)
            {
                Category = GameConsts.LookCategory
            };

            var config = new CommandModifaction<TReturn, LookCommand, RoomBuilder>(command, source.Impl, source.Impl.EventSource);
            commandConfig(config);

            source.Impl.Root.AddBlueprint(
                new RoomCommand(command,
                    LazyString.New(GameConsts.LookAtCommand).AddParameters(source.Id)));
            return source.Impl;
        }

        public static DropItemConfiguration<RoomBuilder, TItem> PickUpCommand<TItem>(this DropItemConfiguration<RoomBuilder, TItem> source, Action<CommandModifaction<TItem, PickupCommand, RoomBuilder>>? config = null) 
            where TItem : ItemBuilder<RoomBuilder>
        {
            var command = new PickupCommand(source.Target.EventSource.Name, source.Target.BluePrint.Id)
            {
                HideOnExecute = true
            };

            ((IWithMetadata)source.Target.EventSource).Metadata["Pickup" + command.GetId] = command;
            source.Target.AddBlueprint(new RoomCommand(command, LazyString.New(GameConsts.PickUpCommand).AddParameters(StringParameter.Resolved(source.Target.BluePrint.Id))));
            config?.Invoke(new CommandModifaction<TItem, PickupCommand, RoomBuilder>(command, source.Target, source.Target.EventSource));
            return source;
        }

        public static DropItemConfiguration<RoomBuilder, TItem> CanDrop<TItem>(this TItem item) 
            where TItem : ItemBuilder<RoomBuilder>
            => new DropItemConfiguration<RoomBuilder, TItem>(item);

        public static TItem WithPoi<TItem>(this TItem builder, PointOfInterst intrest)
            where TItem : ItemBuilder<RoomBuilder>, IHasRoot
        {
            builder.Root.AddBlueprint(new PointOfIntrestAddr(intrest));
            return builder;
        }

        public static TItem WithPoi<TItem>(this TItem builder, Action<PointOfIntrestConfiguration<RoomBuilder, TItem>>? config = null)
            where TItem : ItemBuilder<RoomBuilder>, IHasRoot
        {
            config ??= c => c.WithText(builder.BluePrint.Description);

            var poiBuillder = new PointOfIntrestConfiguration<RoomBuilder, TItem>(builder.EventSource, builder);
            config.Invoke(poiBuillder);

            return builder;
        }

        public static TItem WithAction<TItem>(this TItem builder, string action)
            where TItem : ItemBuilder<RoomBuilder>
        {
            builder.BluePrint.Action = action;
            return builder;
        }

        public static TItem WithDescription<TItem>(this TItem builder, LazyString description)
            where TItem : ItemBuilder<RoomBuilder>
        {
            builder.BluePrint.Description = description;
            return builder;
        }

        public static TItem WithEventTrigger<TItem>(this TItem builder, string? @event)
            where TItem : ItemBuilder<RoomBuilder>
        {
            builder.BluePrint.TriggerEvent = @event;
            return builder;
        }

        public static TItem WithDisplayName<TItem>(this TItem builder, string name)
            where TItem : ItemBuilder
            //where TEventSource : IWithMetadata, IEntityConfiguration
        {
            builder.BluePrint.DisplayName = name;
            return builder;
        }
    }
}