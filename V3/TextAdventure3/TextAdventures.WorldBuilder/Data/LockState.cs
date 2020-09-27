using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data
{
    public sealed class LockState : SingleValueObject<bool>
    {
        public LockState(bool value) 
            : base(value)
        {
        }
    }
}