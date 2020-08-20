using Adventure.GameEngine.Core;

namespace Adventure.GameEngine.Events
{
    public sealed class GotoDirection
    {
        public Direction Direction { get; }

        public string Original { get; }

        public LazyString? Result { get; set; }

        public GotoDirection(Direction target, string original)
        {
            Direction = target;
            Original = original;
        }
    }
}