using Adventure.GameEngine.Core;

namespace Adventure.GameEngine.Systems.Events
{
    public sealed class CommandContentUpdate
    {
        public LazyString? Result { get; }

        public CommandContentUpdate(LazyString? result) => Result = result;
    }
}