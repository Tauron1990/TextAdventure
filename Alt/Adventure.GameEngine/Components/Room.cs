using System.IO;
using Adventure.GameEngine.Persistence;
using EcsRx.Components;

namespace Adventure.GameEngine.Components
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