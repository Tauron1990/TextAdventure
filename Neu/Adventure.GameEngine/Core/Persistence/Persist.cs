using EcsRx.Blueprints;
using EcsRx.Components;
using EcsRx.Entities;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core.Persistence
{
    [PublicAPI]
    public sealed class PersitBlueprint : IBlueprint
    {
        private readonly string _name;

        public PersitBlueprint(string name)
            => _name = name;

        public void Apply(IEntity entity)
            => entity.AddComponents(new IComponent[] {new Persist(_name)});
    }
    public sealed class Persist : IComponent
    {
        public string Name { get; }

        public Persist(string name) => Name = name;
    }
}