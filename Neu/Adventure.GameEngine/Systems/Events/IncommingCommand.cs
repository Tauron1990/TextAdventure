using Adventure.GameEngine.Commands;

namespace Adventure.GameEngine.Systems.Events
{
    public sealed class IncommingCommand
    {
        public Command Command { get; }

        public IncommingCommand(Command command)
            => Command = command;
    }
}