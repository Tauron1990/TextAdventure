using Adventure.GameEngine.BuilderAlt.ItemData;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.BuilderAlt
{
    [PublicAPI]
    public static class ItemBuilderExtension
    {
        public static TReturn WithLook<TReturn>(this IRoomItemBuilder<TReturn> source, LazyString? respond = null)
        {
            source.Room.Blueprints.Add(
                new RoomCommand(
                    new LookCommand(source.Id)
                    {
                        Responsd = respond,
                        Category = GameConsts.LookCategory
                    },
                    LazyString.New(GameConsts.LookAtCommand).AddParameters(source.Id)));
            return source.Impl;
        }

        public static TReturn WithLook<TReturn>(this IRoomItemBuilder<TReturn> source, LazyString? respond, out LookCommand command)
        {
            source.Room.Blueprints.Add(
                new RoomCommand(
                    command = new LookCommand(source.Id)
                    {
                        Responsd = respond,
                        Category = GameConsts.LookCategory
                    },
                    LazyString.New(GameConsts.LookAtCommand).AddParameters(source.Id)));
            return source.Impl;
        }
    }
}