using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    public sealed class TravelCommand : Command<TravelCommand>
    {
        public Direction Direction { get; }

        public TravelCommand(Direction direction) 
            : base(nameof(TravelCommand)) =>
            Direction = direction;
    }
}