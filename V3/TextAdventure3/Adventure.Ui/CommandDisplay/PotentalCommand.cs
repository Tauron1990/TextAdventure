using System.Windows.Input;
using TextAdventures.Builder.Data.Command;

namespace Adventure.Ui.CommandDisplay
{
    public sealed class PotentalCommand : ICommandContent
    {
        public IGameCommand Command { get; }

        public string Name { get; }

        public ICommand Executor { get; }

        public PotentalCommand(IGameCommand command, string name, ICommand executor)
        {
            Command = command;
            Name = name;
            Executor = executor;
        }
    }
}