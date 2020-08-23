namespace Adventure.GameEngine.Systems.Events
{
    public sealed class TriggerEvent
    {
        public string Name { get; }

        public TriggerEvent(string name)
            => Name = name;
    }
}