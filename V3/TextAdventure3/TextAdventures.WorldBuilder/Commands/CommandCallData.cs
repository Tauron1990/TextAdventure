using Akkatecture.ValueObjects;
using JetBrains.Annotations;

namespace TextAdventures.Builder.Commands
{
    [PublicAPI]
    public enum CallType
    {
        UI,
        Update,
        Internal
    }

    [PublicAPI]
    public sealed class CommandCallData : ValueObject
    {
        public CommandCallData(CallType callType, int index = -1)
        {
            CallType = callType;
            Index    = index;
        }

        public CallType CallType { get; }

        public int Index { get; }
    }
}