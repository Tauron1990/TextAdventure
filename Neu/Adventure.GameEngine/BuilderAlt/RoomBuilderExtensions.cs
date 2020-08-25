using System;
using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.BuilderAlt.ItemData;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.BuilderAlt
{
    [PublicAPI]
    public static class RoomBuilderExtensions
    {
        public static RoomBuilder WithDescription(this RoomBuilder builder, string description)
            => builder.WithBluePrint(new RoomDescription(description));

        public static RoomBuilder WithDropItem(this RoomBuilder room, string id, Action<DropItemBuilder> builder)
        {
            var temp = new DropItemBuilder(room, id, room.Config);
            builder(temp);
            return room;
        }

        public static RoomBuilder WithDropItem(this RoomBuilder room, ItemBuilder item, Action<DropItemBuilder>? builder = null)
        {
            var temp = new DropItemBuilder(room, item, room.Config);
            builder?.Invoke(temp);
            return room;
        }

        public static RoomBuilder WithInteriorItem(this RoomBuilder room, string id, Action<SimpleItemBuilder> builder)
        {
            var temp = new SimpleItemBuilder(room, id, room.Config);
            builder(temp);
            return room;
        }

        public static RoomBuilder WithInteriorItem(this RoomBuilder room, ItemBuilder item, Action<SimpleItemBuilder>? builder = null)
        {
            var temp = new SimpleItemBuilder(room, item, room.Config);
            builder?.Invoke(temp);
            return room;
        }

        public static RoomBuilder WithCommand<TCommand>(this RoomBuilder builder, LazyString name, TCommand command, Action<CommandModifaction<RoomBuilder, TCommand>>? config = null)
            where TCommand : Command
        {
            builder.Blueprints.Add(new RoomCommand(command, name));
            config?.Invoke( new CommandModifaction<RoomBuilder, TCommand>(command, builder));
            return builder;
        }

        public static RoomBuilder WithCommand(this RoomBuilder builder, LazyString name, CommandId id, Action<CommandModifaction<RoomBuilder, Command>>? config = null)
        {
            var com = builder.Config.GetCommand(id);
            builder.Blueprints.Add(new RoomCommand(com, name));
            config?.Invoke(new CommandModifaction<RoomBuilder, Command>(com, builder));
            return builder;
        }

        public static RoomBuilder ForceTravel(this RoomBuilder builder, LazyString name, RoomBuilder target)
        {
            builder.Blueprints.Add(new RoomCommand(
                new ForceTravelTo(target.Name)
                {
                    Category = GameConsts.TravelCategory
                },
                name));

            return builder;
        }

        public static RoomBuilder ForceTravel(this RoomBuilder builder, LazyString name, string target)
        {
            builder.Blueprints.Add(new RoomCommand(
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