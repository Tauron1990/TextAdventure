using System;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Internal.Data.Aggregates
{
    public sealed class GameInfoId : Identity<GameInfoId>
    {
        public static readonly Guid GameId = new("B989CA41-C4F4-4E71-A421-8D0477DEE97D");

        public static readonly GameInfoId Id = With(GameId);

        public GameInfoId(string value) : base(value) { }
    }

    public sealed class GameInfoState : GameState<GameInfoState, GameInfo, GameInfoId>
    {
        public override void Hydrate(GameInfoState aggregateSnapshot) { }
    }

    public sealed class GameInfo : GameAggregate<GameInfo, GameInfoId, GameInfoState>
    {
        public GameInfo(GameInfoId id)
            : base(id) { }
    }

    public sealed class GameInfoManager : AggregateManager<GameInfo, GameInfoId, GameInfoCommand> { }
}