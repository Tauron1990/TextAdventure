using Akkatecture.ValueObjects;

namespace TextAdventures.Builder.Data
{
    public sealed class Name : SingleValueObject<string>
    {
        public static readonly Name Default = new Name("");

        public Name(string value) 
            : base(value)
        {
        }
    }
}