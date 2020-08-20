using Adventure.GameEngine.Core;

namespace Adventure.GameEngine.Events
{
    public sealed class CommandExecutionCompled
    {
        public LazyString? Result { get; }

        public CommandExecutionCompled(LazyString? result) => Result = result;
    }
}