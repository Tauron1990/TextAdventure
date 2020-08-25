using Adventure.GameEngine.BuilderAlt;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.RoomData
{
    [PublicAPI]
    public sealed class PrologBuilder : IRoomFactory
    {
        private readonly string _description;

        private RoomBuilder? _forceTravel;
        private string _displayName = string.Empty;

        public PrologBuilder(string description)
            => _description = description;

        public PrologBuilder WithStart(RoomBuilder builder, string displayName)
        {
            _forceTravel = builder;
            _displayName = displayName;
            return this;
        }

        public string Name => "Prolog";

        public RoomBuilder Apply(RoomBuilder builder, GameConfiguration gameConfiguration)
        {
            builder = builder.WithDescription(_description);

            if (_forceTravel != null)
                builder = builder.ForceTravel(_displayName, _forceTravel);

            return builder;
        }
    }
}