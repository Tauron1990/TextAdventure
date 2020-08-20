using Adventure.TextProcessing.Interfaces;
using DefaultEcs;

namespace Adventure.GameEngine.Systems
{
    public sealed class CommandInfo
    {
        public Entity Room { get; }

        public ICommand Command { get; }

        public string Result { get; set; }

        public CommandInfo(Entity room, ICommand command)
        {
            Room = room;
            Command = command;
        }
    }
}