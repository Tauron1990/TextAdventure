namespace TextAdventures.Engine.Data
{
    public sealed class ComponentObject
    {
        public object Component { get; }

        public ComponentObject(object component) => Component = component;
    }
}