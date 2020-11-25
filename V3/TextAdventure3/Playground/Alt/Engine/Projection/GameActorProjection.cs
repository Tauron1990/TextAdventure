using TextAdventures.Builder.Data;
using TextAdventures.Builder.Data.Actor;
using TextAdventures.Engine.Projection.Base;

namespace TextAdventures.Engine.Projection
{
    public class GameActorProjection : ProjectionBase<GameActorId>
    {
        public PlayerType PlayerType
        {
            get => GetItem(PlayerType.Default);
            set => SetItem(value);
        }

        public Name Name
        {
            get => GetItem(Name.Default);
            set => SetItem(value);
        }

        public Name DisplayName
        {
            get => GetItem(Name.Default);
            set => SetItem(value);
        }

        public ActorLocation Location
        {
            get => GetItem(ActorLocation.Unkowen);
            set => SetItem(value);
        }
    }
}