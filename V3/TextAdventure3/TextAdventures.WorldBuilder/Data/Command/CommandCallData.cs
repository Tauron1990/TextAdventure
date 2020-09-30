using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data.Command
{
    public enum CallType
    {
        UI,
        Update,
        Internal
    }

    public sealed class CommandCallData : ValueObject
    {
        public CallType CallType { get; }

        public int Index { get; }

        public CommandCallData(CallType callType, int index = -1)
        {
            CallType = callType;
            Index = index;
        }
    }
}