using System.Collections.Generic;
using System.Collections.Immutable;
using EcsRx.Blueprints;
using EcsRx.Collections.Database;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Builder.Core
{
    public abstract class EntityConfiguration : IEntityConfiguration, IWithMetadata
    {
        private ImmutableArray<IBlueprint> _blueprints = ImmutableArray<IBlueprint>.Empty;

        ImmutableArray<IBlueprint> IEntityConfiguration.Data => _blueprints;

        void IEntityConfiguration.AddBlueprint(IBlueprint blueprint)
        {
            _blueprints = _blueprints.Add(blueprint);
        }

        void IEntityConfiguration.Create(IEntityDatabase database)
        {
            database.GetCollection().CreateEntity(_blueprints);
            CreateVirtual(database);
        }

        protected virtual void CreateVirtual(IEntityDatabase database) { }

        protected virtual void ValidateImpl() { }

        void IEntityConfiguration.Validate()
            => ValidateImpl();


        protected abstract Dictionary<string, object> Metadata { get; }

        Dictionary<string, object> IWithMetadata.Metadata => Metadata;
    }
}