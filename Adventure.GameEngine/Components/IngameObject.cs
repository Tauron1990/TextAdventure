using System.IO;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Persistence;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Adventure.GameEngine.Components
{
    public sealed class IngameObject : IComponent, IPersistComponent
    {
        public string Id { get; }

        public ReactiveProperty<string> Location { get; }

        public LazyString Description { get; set; }

        public string DisplayName { get; }

        public IngameObject(string id, LazyString description, ReactiveProperty<string> location, string displayName)
        {
            Id = id;
            Description = description;
            Location = location;
            DisplayName = displayName;
        }

        string IPersistComponent.Id => "InGameObject";

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Location.Value);
            BinaryHelper.Write(writer, Description);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Location.Value = reader.ReadString();
            Description = BinaryHelper.Read<LazyString>(reader);
        }
    }
}