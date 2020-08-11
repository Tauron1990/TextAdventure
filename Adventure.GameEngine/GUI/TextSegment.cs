using JetBrains.Annotations;

namespace Adventure.GameEngine.GUI
{
    [PublicAPI]
    public sealed class TextSegment
    {
        public string Text { get; }

        public UiPosition Start { get; }

        public UiPosition End { get; }

        public TextSegment(string text, UiPosition start, UiPosition end)
        {
            Text = text;
            Start = start;
            End = end;
        }

        public TextSegment UpdateText(string newText) 
            => new TextSegment(newText, Start, End);

        public TextSegment UpdateStart(ref UiPosition position) 
            => new TextSegment(Text, position, UiPosition.Invalid);

        public TextSegment PrintAndUpdate(IConsole console)
        {
            console.CursorLeft = Start.Left;
            console.CursorTop = Start.Top;

            console.Write(Text);
            return new TextSegment(Text, Start, new UiPosition(console.CursorTop, console.CursorLeft));
        }
    }
}