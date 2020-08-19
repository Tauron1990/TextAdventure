using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Adventure.GameEngine.Persistence;
using EcsRx.Components;
namespace Adventure.GameEngine.Components
{
    public sealed class ReplayInfo : IComponent, IPersistComponent
    {
        public ImmutableList<string> Commands { get; private set; }

        public ReplayInfo(ImmutableList<string> commands) => Commands = commands;

        public ReplayInfo() => Commands = ImmutableList<string>.Empty;

        public void Add(string command)
        {
            Commands = Commands.Add(command);
        }

        void IPersitable.WriteTo(BinaryWriter writer)
            => BinaryHelper.WriteList(Commands, writer);

        void IPersitable.ReadFrom(BinaryReader reader)
            => Commands = BinaryHelper.ReadString(reader).ToImmutableList();

        string IPersistComponent.Id => "ReplayInfo";
    }
}