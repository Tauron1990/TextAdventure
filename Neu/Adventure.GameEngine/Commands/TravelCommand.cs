using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    public abstract class TravelCommand : Command<TravelCommand>
    {
        protected TravelCommand([NotNull] string id) : base(id)
        {
        }
    }

    public sealed class ForceTravelTo : TravelCommand
    {
        public string Room { get; }

        public ForceTravelTo(string room) : base(nameof(ForceTravelTo))
            => Room = room;
    }

    public sealed class DirectionTravelCommand : TravelCommand
    {
        public Direction Direction { get; }

        public DirectionTravelCommand(Direction direction)
            : base(nameof(TravelCommand)) =>
            Direction = direction;
    }
}