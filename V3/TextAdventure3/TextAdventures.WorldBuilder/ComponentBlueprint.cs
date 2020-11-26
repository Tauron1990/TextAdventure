using System;

namespace TextAdventures.Builder
{
    public sealed class ComponentBlueprint
    {
        public Type ComponentType { get; }

        public ComponentBlueprint(Type componentType) => ComponentType = componentType;
    }
}