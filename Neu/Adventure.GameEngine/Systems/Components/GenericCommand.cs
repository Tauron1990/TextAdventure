using System.IO;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class GenericCommand : IComponent, IPersistComponent
    {
        public Command Command { get; }

        public GenericCommand(Command command)
            => Command = command;

        public void WriteTo(BinaryWriter writer)
            => BinaryHelper.Write(writer, Command);

        public void ReadFrom(BinaryReader reader)
            => BinaryHelper.Read(reader, () => Command);

        public string Id => nameof(GenericCommand);
    }
}