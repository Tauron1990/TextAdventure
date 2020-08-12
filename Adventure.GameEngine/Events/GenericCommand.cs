using Adventure.TextProcessing.Interfaces;

namespace Adventure.GameEngine.Events
{
    public sealed class GenericCommand
    {
        public ICommand Command { get; }

        public string? Result { get; set; }

        public GenericCommand(ICommand command)
        {
            Command = command;
        }
    }
}