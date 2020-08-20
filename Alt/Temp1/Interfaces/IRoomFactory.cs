using System.Collections.Generic;
using Adventure.GameEngine.Systems;
using DefaultEcs;
using DefaultEcs.System;

namespace Adventure.GameEngine.Interfaces
{
    public interface IRoomFactory
    {
        void Initialize(Entity entity);

        IRoom Create(Entity ent);

        ISystem<CommandInfo> CreateCommandRunner(Entity ent);

        void Connect(Entity room, IEnumerable<Entity> other);
    }
}