using System.IO;
using Adventure.GameEngine.Persistence;
using EcsRx.Components;

namespace Adventure.GameEngine.Components
{
    public sealed class Room : IComponent, IPersistComponent
    {
        public string Name { get; private set; }

        public Room()
        {
            
        }

        public Room(string name) 
            => Name = name;

        void IPersitable.WriteTo(BinaryWriter writer)
            => writer.Write(Name);

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Name = reader.ReadString();
        }

        string IPersistComponent.Id => "Room";
    }
}