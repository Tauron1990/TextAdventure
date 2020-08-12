using EcsRx.Components;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class Room : IComponent
    {
        public string Name { get; }

        public Room()
        {
            
        }

        [JsonConstructor]
        public Room(string name) 
            => Name = name;
    }
}