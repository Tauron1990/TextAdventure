using Adventure.GameEngine.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Rooms
{
    [PublicAPI]
    public static class RoomBuilderExtensions
    {
        public static RoomBuilder WithCommonCommandSet(this RoomBuilder builder)
            => builder.WithBluePrint(builder.CommonCommands.CreateCommonCommandBlueprint());

        public static RoomBuilder WithDescription(this RoomBuilder builder, string description) 
            => builder.WithBluePrint(new RoomDescription(description));

        public static DropItemBuilder WithDropItem(this RoomBuilder builder, string id)
            => new DropItemBuilder(builder, id);

        public static DropItemBuilder WithDropItem(this RoomBuilder builder, ItemBuilder item)
            => new DropItemBuilder(builder, item);
    }
}