using System;

namespace TextAdventures.Engine.Data
{
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