using System;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace TextAdventures.Builder
{
    [PublicAPI]
    public sealed class ComponentBlueprint
    {
        private readonly GameObjectBlueprint? _gameObjectBlueprint;

        public ComponentBlueprint(Type componentType, GameObjectBlueprint? gameObjectBlueprint)
        {
            _gameObjectBlueprint = gameObjectBlueprint;
            ComponentType = componentType;
        }

        public Type ComponentType { get; }

        public ImmutableDictionary<string, object?> DefaultValues { get; private set; } =
            ImmutableDictionary<string, object?>.Empty;

        public static ComponentBlueprint Single<TComponent>()
            where TComponent : IComponent
            => new(typeof(TComponent), null);

        public ComponentBlueprint AddDefaultValue(string property, object? value)
        {
            DefaultValues = DefaultValues.SetItem(property, value);
            return this;
        }

        public GameObjectBlueprint And()
            => _gameObjectBlueprint ?? throw new InvalidOperationException("Uncoopled Component Blueprint");

        public static implicit operator GameObjectBlueprint(ComponentBlueprint blueprint)
            => blueprint.And();
    }
}