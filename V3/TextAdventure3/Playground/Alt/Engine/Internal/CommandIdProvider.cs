using Akkatecture.Commands;

namespace TextAdventures.Engine.Internal
{
    public sealed class CommandIdProvider
    {
        public static readonly CommandId Current = CommandId.NewComb();
    }
}