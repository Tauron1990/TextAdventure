using Adventure.GameEngine.Commands;

namespace Adventure.GameEngine.Builder.CommandData
{
    public sealed class CommandModifaction<TReturn, TCommand>
        where TCommand : Command
    {
        public TCommand Command { get; }

        public TReturn Info { get; }

        public CommandModifaction(TCommand command, TReturn info)
        {
            Command = command;
            Info = info;
        }

        public TReturn Return()
            => Info;
    }
}