using System.IO;

namespace Adventure.GameEngine.Persistence
{
    public interface IPersitable
    {
        void WriteTo(BinaryWriter writer);

        void ReadFrom(BinaryReader reader);
    }

    public interface IPersistComponent : IPersitable
    {
        public string Id { get; }
    }
}