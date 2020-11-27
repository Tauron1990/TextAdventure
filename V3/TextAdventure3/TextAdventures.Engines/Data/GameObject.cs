using System;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace TextAdventures.Engine.Data
{
    [PublicAPI]
    public interface IGameObject
    {
        bool HasComponent<T>();

        bool HasComponent(Type component);

        T GetComponent<T>();

        object GetComponent(Type component);
    }

    public sealed class GameObject : IGameObject
    {
        public string Name { get; }

        public ImmutableList<ComponentObject> Components { get; }


        public GameObject(string name, ImmutableList<ComponentObject> components)
        {
            Name       = name;
            Components = components;
        }

        public bool HasComponent<T>() => HasComponent(typeof(T));

        public bool HasComponent(Type component) => Components.Any(c => c.ComponentType == component);

        public T GetComponent<T>() => (T)GetComponent(typeof(T));

        public object GetComponent(Type component) => Components.First(c => c.ComponentType == component);
    }
}