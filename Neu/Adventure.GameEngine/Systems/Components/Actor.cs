using System.IO;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class Actor : IComponent, IPersistComponent
    {
        public ReactiveProperty<string> Location { get; }

        public string Name { get; set; } = "Unbennant";

        public Actor(string location) 
            => Location = new ReactiveProperty<string>(location);

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Location.Value);
            writer.Write(Name);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Location.Value = reader.ReadString();
            Name = reader.ReadString();
        }

        string IPersistComponent.Id => "Actor";
    }
}