using System.IO;

namespace Adventure.GameEngine.Core.Persistence
{
    public interface IPersitable
    {
        void WriteTo(BinaryWriter writer);

        void ReadFrom(BinaryReader reader);
    }
}