using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Collections.Database;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Builder.Core
{
    public abstract class SubEntityConfiguration : EntityConfiguration, IWithSubEntity
    {
        private ImmutableDictionary<string, IBluePrintProvider> _subEntities = ImmutableDictionary<string, IBluePrintProvider>.Empty;

        protected SubEntityConfiguration(Dictionary<string, object> metadata)
            => Metadata = metadata;

        protected override Dictionary<string, object> Metadata { get; }

        ImmutableDictionary<string, IBluePrintProvider> IWithSubEntity.SubEntities => _subEntities;

        void IWithSubEntity.AddToSubSubEntity(string name, IBluePrintProvider blueprint)
            => _subEntities = _subEntities.Add(name, blueprint);

        protected override void CreateVirtual(IEntityDatabase database)
        {
            foreach (var entity in _subEntities)
                database.GetCollection().CreateEntity(entity.Value.Blueprints);
            base.CreateVirtual(database);
        }

        protected override void ValidateImpl()
        {
            foreach (var provider in _subEntities.Select(e => e.Value))
                provider.Validate();

            base.ValidateImpl();
        }
    }
}