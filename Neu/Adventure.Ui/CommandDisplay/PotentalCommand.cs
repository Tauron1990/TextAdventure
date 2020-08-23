using System.Windows.Input;
using Adventure.GameEngine.Commands;

namespace Adventure.Ui.CommandDisplay
{
    public sealed class PotentalCommand : ICommandContent
    {
        public Command Command { get; }

        public string Name { get; }

        public ICommand Executor { get; }

        public PotentalCommand(Command command, string name, ICommand executor)
        {
            Command = command;
            Name = name;
            Executor = executor;
        }
    }
}