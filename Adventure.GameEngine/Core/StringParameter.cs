using Adventure.Utilities.Interfaces;

namespace Adventure.GameEngine.Core
{
    public sealed class StringParameter
    {
        public bool Resolve { get; }

        public string Text { get; }

        public StringParameter(bool resolve, string text)
        {
            Resolve = resolve;
            Text = text;
        }

        public string Format(IContentManagement management) 
            => Resolve ? management.RetrieveContentItem(Text, Text) : Text;

        public static StringParameter Resolved(string data)
            => new StringParameter(true, data);

        public static StringParameter FromText(string data)
            => new StringParameter(false, data);
    }
}