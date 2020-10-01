using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data
{
    public sealed class Description : SingleValueObject<string>
    {
        public Description(string value) : base(value)
        {
        }
    }
}