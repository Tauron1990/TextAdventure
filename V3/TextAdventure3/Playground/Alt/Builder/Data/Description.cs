using Akkatecture.ValueObjects;
using JetBrains.Annotations;

namespace TextAdventures.Builder.Data
{
    [PublicAPI]
    public sealed class Description : SingleValueObject<string>
    {
        public Description(string value) : base(value) { }
    }
}