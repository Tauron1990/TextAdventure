using System;
using Adventure.GameEngine.Core.Persistence;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class BaseGameInfo : IBlueprint
    {
        private readonly int _version;

        public BaseGameInfo(int version) 
            => _version = version;

        public void Apply(IEntity entity)
        {
            entity.AddComponents(new GameInfo(_version, DateTimeOffset.UtcNow, string.Empty, string.Empty), new Persist(nameof(BaseGameInfo)));
        }
    }
}