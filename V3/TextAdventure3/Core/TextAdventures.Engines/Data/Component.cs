using System;

namespace TextAdventures.Engine.Data
{
    public sealed record RequestGameObject(string Name);

    public sealed record RequestGameComponent(Type Type);

    public sealed record RespondGameObject(GameObject? GameObject);

    public sealed record RespondGameComponent(object? Type);

    public sealed class ComponentObject
    {
        public object Component { get; }

        public Type ComponentType { get; }

        public ComponentObject(object component)
        {
            Component = component;
            ComponentType = component.GetType();
        }
    }
}