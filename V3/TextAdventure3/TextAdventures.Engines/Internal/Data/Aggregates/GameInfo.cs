using Akkatecture.Aggregates;
using Akkatecture.Core;
using Newtonsoft.Json;
using TextAdventures.Engine.Internal.Data.Commands;

namespace TextAdventures.Engine.Internal.Data.Aggregates
{
    public sealed class GameInfoId : Identity<GameInfoId>
    {
        public const string GameId = "gameinfo-B989CA41-C4F4-4E71-A421-8D0477DEE97D";

        [JsonConstructor]
        private GameInfoId(string value) : base(value)
        { }

        public GameInfoId()
            : this(GameId)
        {
            
        }
    }

    public sealed class GameInfoState : GameState<GameInfoState, GameInfo, GameInfoId>
    {
        public override void Hydrate(GameInfoState aggregateSnapshot)
        {
            
        }
    }

    public sealed class GameInfo : GameAggregate<GameInfo, GameInfoId, GameInfoState>
    {
        public GameInfo(GameInfoId id) 
            : base(id)
        {
        }
    }

    public sealed class GameInfoManager : AggregateManager<GameInfo, GameInfoId, GameInfoCommand>
    {

    }
}