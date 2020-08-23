using Adventure.GameEngine.Core;

namespace Adventure.GameEngine.Commands
{
    public sealed class UseCommand : Command<UseCommand>
    {
        public UseCommand() 
            : base(nameof(UseCommand)) { }

        public LazyString Respond { get; }
    }
}