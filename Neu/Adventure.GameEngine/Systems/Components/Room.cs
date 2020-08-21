using EcsRx.Components;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class Room : IComponent
    {
        public string Name { get; }

        public Room()
        {
            
        }

        public Room(string name) 
            => Name = name;
    }
}