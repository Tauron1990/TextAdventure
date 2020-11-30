using System.Windows.Input;

namespace Adventure.Ui.CommandDisplay
{
    public sealed class PotentalCommand : ICommandContent
    {
        public IGameCommand Command { get; }

        public ICommand Executor { get; }

        public PotentalCommand(IGameCommand command, string name, ICommand executor)
        {
            Command = command;
            Name = name;
            Executor = executor;
        }

        public string Name { get; }
    }
}