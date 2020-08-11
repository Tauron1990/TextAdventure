using EcsRx.Components;

namespace Adventure.GameEngine.Components
{
    public sealed class IsPlayerControlled : IComponent
    {
        public string Name { get; set; }

        public IsPlayerControlled(string name)
        {
            Name = name;
        }
    }
}