using System;
using Adventure.GameEngine.Builder.RoomData;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public static class RoomFactory
    {
        public static RoomBuilder AddRoom(this GameConfiguration config, IRoomFactory factory)
            => factory.Apply(config.Rooms.NewRoom(factory.Name), config);

        public static RoomBuilder AddRoom<TRoom>(this GameConfiguration config)
            where TRoom : IRoomFactory, new()
            => AddRoom(config, new TRoom());

        public static RoomBuilder AddProlog(this GameConfiguration config, string prolog, Action<PrologBuilder> prologbuilder)
        {
            var builder = new PrologBuilder(prolog);
            prologbuilder(builder);
            return AddRoom(config, builder);
        }
    }
}