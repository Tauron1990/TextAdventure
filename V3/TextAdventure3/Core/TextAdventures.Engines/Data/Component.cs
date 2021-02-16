using System;

namespace TextAdventures.Engine.Data
{
    public sealed record RequestGameObject(string Name);

    public sealed record RequestGameComponent(Type Type);

    public sealed record RespondGameObject(GameObject? GameObject);

    public sealed record RespondGameComponent(object? Component);

    public sealed class ComponentObject
    {
        public ComponentObject(object? component)
        {
            Component = component ?? throw new ArgumentNullException(nameof(component));
            ComponentType = component.GetType();
        }

        public object Component { get; }

        public Type ComponentType { get; }
    }
}