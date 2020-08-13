namespace Adventure.GameEngine.Events
{
    public sealed class GotoDirection
    {
        public Direction Direction { get; }

        public string? Result { get; set; }

        public GotoDirection(Direction target) => Direction = target;
    }
}