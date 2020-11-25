using Akkatecture.ValueObjects;
using JetBrains.Annotations;

namespace TextAdventures.Builder.Data
{
    [PublicAPI]
    public sealed class LockState : SingleValueObject<bool>
    {
        public LockState(bool value)
            : base(value) { }
    }
}