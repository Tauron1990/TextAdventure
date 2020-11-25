using Akkatecture.Aggregates;
using Akkatecture.Events;
using TextAdventures.Builder.Data.Actor;

namespace TextAdventures.Engine.Processors.Commands
{
    [EventVersion("PlayerRecivedEvent", 1)]
    public sealed class PlayerRecivedEvent : AggregateEvent<CommandTracker, CommandTrackerId>
    {
        public PlayerRecivedEvent(GameActorId player) => Player = player;
        public GameActorId Player { get; }
    }
}