using System;
using System.Collections.Immutable;
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

    public sealed record GameObject(string Name, ImmutableDictionary<Type, ComponentObject> Components) : IGameObject
    {
        public bool HasComponent<T>() => HasComponent(typeof(T));

        public bool HasComponent(Type component) => Components.ContainsKey(component);

        public T GetComponent<T>() => (T) GetComponent(typeof(T));

        public object GetComponent(Type component) => Components[component].Component;
    }
}