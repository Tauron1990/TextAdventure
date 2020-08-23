using System.Collections.Immutable;
using System.IO;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    [PublicAPI]
    public sealed class RoomCommands : IComponent, IPersistComponent
    {

        public ImmutableDictionary<LazyString, Command> Commands { get; private set; } = ImmutableDictionary<LazyString, Command>.Empty;

        public RoomCommands()
        { }

        public void Add(string name, Command handler)
            => Commands = Commands.Add(LazyString.New(name), handler);

        public void Add(LazyString name, Command handler)
            => Commands = Commands.Add(name, handler);

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Commands.Count);

            foreach (var command in Commands)
            {
                BinaryHelper.Write(writer, command.Key);
                BinaryHelper.Write(writer, command.Value);
            }
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            for (int i = 0; i < reader.ReadInt32(); i++)
            {
                var key = BinaryHelper.Read(reader, LazyString.New);
                if (Commands.TryGetValue(key, out var command))
                    BinaryHelper.Read(reader, () => command);
            }
        }

        string IPersistComponent.Id => nameof(RoomCommands);
    }
}