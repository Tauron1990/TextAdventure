using System.IO;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Persistence;
using EcsRx.Components;

namespace Adventure.GameEngine.Components
{
    public sealed class GenericCommandDescription : IComponent, IPersistComponent
    {
        public string Name { get; private set; }

        public LazyString Description { get; set; }

        public GenericCommandDescription()
        {
            
        }

        public GenericCommandDescription(string name, LazyString description)
        {
            Name = name;
            Description = description;
        }

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Name);
            BinaryHelper.Write(writer, Description);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Name = reader.ReadString();
            Description = BinaryHelper.Read<LazyString>(reader);
        }

        string IPersistComponent.Id => "GenericCommandDescription";
    }
}