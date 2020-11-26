using System.Collections.Immutable;

namespace TextAdventures.Engine.Data
{
    public sealed class GameObject
    {
        public string Name { get; }

        public ImmutableList<ComponentObject> Components { get; }

        public GameObject(string name, ImmutableList<ComponentObject> components)
        {
            Name       = name;
            Components = components;
        }
    }
}