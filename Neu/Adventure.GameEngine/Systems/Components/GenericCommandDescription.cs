using System.IO;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    [PublicAPI]
    public sealed class GenericCommandDescription : IComponent, IPersistComponent
    {
        public string Name { get; private set; } = string.Empty;

        public LazyString Description { get; set; } = LazyString.New();

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
            Description = BinaryHelper.Read(reader, LazyString.New);
        }

        string IPersistComponent.Id => "GenericCommandDescription";
    }
}