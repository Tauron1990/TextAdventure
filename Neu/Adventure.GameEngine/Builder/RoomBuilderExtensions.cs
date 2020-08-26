using System;
using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.ItemData;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public static class RoomBuilderExtensions
    {
        public static TType WithBlueprint<TType>(this TType configuration, IBlueprint blueprint)
            where TType : IEntityConfiguration
        {
            configuration.AddBlueprint(blueprint);
            return configuration;
        }

        public static RoomBuilder WithDescription(this RoomBuilder builder, string description)
            => builder.WithBlueprint(new RoomDescription(description));

        public static RoomBuilder WithItem(this RoomBuilder room, string id, Action<SimpleItemBuilder<RoomBuilder>> builder)
        {
            var temp = new SimpleItemBuilder<RoomBuilder>(room, id, room);
            builder(temp);
            return room;
        }

        public static RoomBuilder WithItem(this RoomBuilder room, ItemBuilder item, Action<SimpleItemBuilder<RoomBuilder>>? builder = null)
        {
            var temp = new SimpleItemBuilder<RoomBuilder>(room, item, room);
            builder?.Invoke(temp);
            return room;
        }

        public static RoomBuilder WithCommand<TCommand>(this RoomBuilder builder, LazyString name, TCommand command, Action<CommandModifaction<RoomBuilder, TCommand, RoomBuilder>>? config = null)
            where TCommand : Command
        {
            builder.WithBlueprint(new RoomCommand(command, name));
            config?.Invoke( new CommandModifaction<RoomBuilder, TCommand, RoomBuilder>(command, builder, builder));
            return builder;
        }

        public static RoomBuilder ForceTravel(this RoomBuilder builder, LazyString name, RoomBuilder target)
        {
            builder.WithBlueprint(new RoomCommand(
                new ForceTravelTo(target.Name)
                {
                    Category = GameConsts.TravelCategory
                },
                name));

            return builder;
        }

        public static RoomBuilder ForceTravel(this RoomBuilder builder, LazyString name, string target)
        {
            builder.WithBlueprint(new RoomCommand(
                new ForceTravelTo(target)
                {
                    Category = GameConsts.TravelCategory
                },
                name));

            return builder;
        }

        public static RoomBuilder WithDisplayName(this RoomBuilder builder, string displayName)
        {
            builder.ContentManagement.AddContentItem(builder.Name, displayName);
            return builder;
        }
    }
}