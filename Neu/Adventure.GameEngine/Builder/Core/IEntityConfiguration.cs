using System.Collections.Immutable;
using EcsRx.Blueprints;
using EcsRx.Collections.Database;

namespace Adventure.GameEngine.Builder.Core
{
    public interface IEntityConfiguration
    {
        ImmutableArray<IBlueprint> Data { get; }

        void AddBlueprint(IBlueprint blueprint);

        void Create(IEntityDatabase database);

        void Validate();
    }
}