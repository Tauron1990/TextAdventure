using System.IO;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class IngameObject : IComponent, IPersistComponent
    {
        public string Id { get; }

        public ReactiveProperty<string> Location { get; }

        public ReactiveProperty<LazyString> Description { get; }

        public string DisplayName { get; }

        public IngameObject(string id, LazyString description, string location, string displayName)
        {
            Id = id;
            Description = new ReactiveProperty<LazyString>(description);
            Location = new ReactiveProperty<string>(location);
            DisplayName = displayName;
        }

        string IPersistComponent.Id => "InGameObject";

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Location.Value);
            BinaryHelper.Write(writer, Description.Value);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Location.Value = reader.ReadString();
            Description.Value = BinaryHelper.Read(reader, LazyString.New);
        }
    }
}