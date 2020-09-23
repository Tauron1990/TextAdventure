using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data
{
    public sealed class Name : SingleValueObject<string>
    {
        public Name(string value) 
            : base(value)
        {
        }
    }
}