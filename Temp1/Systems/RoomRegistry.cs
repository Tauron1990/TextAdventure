using System.Collections.Generic;
using Adventure.GameEngine.Interfaces;
using DefaultEcs;
using DefaultEcs.System;

namespace Adventure.GameEngine.Systems
{
    public sealed class RoomRegistry
    {
        private sealed class SelectionSystem : ISystem<CommandInfo>
        {
            private readonly Dictionary<Entity, ISystem<CommandInfo>> _handler;

            public SelectionSystem(Dictionary<Entity, ISystem<CommandInfo>> handler)
                => _handler = handler;


            public void Dispose()
            {
                foreach (var system in _handler) 
                    system.Value.Dispose();

                _handler.Clear();
            }

            public void Update(CommandInfo state)
            {
                if(!IsEnabled) return;

                if(_handler.TryGetValue(state.Room, out var handler) && handler.IsEnabled)
                    handler.Update(state);
            }

            public bool IsEnabled { get; set; } = true;
        }

        private readonly IEnumerable<IRoomFactory> _factories;
        private readonly IGame _game;

        public RoomRegistry(IEnumerable<IRoomFactory> factories, IGame game)
        {
            _factories = factories;
            _game = game;
        }

        public ISystem<CommandInfo> Init()
        {
            var entDic = new Dictionary<Entity, ISystem<CommandInfo>>();
            var toConnect = new Dictionary<Entity, IRoomFactory>(); 

            foreach (var factory in _factories)
            {
                var ent = _game.World.CreateEntity();
                ent.Set(_game);
                ent.Set(factory.Create(ent));
                factory.Initialize(ent);

                entDic[ent] = factory.CreateCommandRunner(ent);
                toConnect[ent] = factory;
            }

            foreach (var factory in toConnect) 
                factory.Value.Connect(factory.Key, entDic.Keys);

            return new SelectionSystem(entDic);
        }
    }
}