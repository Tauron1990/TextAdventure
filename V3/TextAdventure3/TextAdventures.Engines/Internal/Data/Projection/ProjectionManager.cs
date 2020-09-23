using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using TextAdventures.Engine.Data;
using TextAdventures.Engine.Internal.Data.Aggregates;
using TextAdventures.Engine.Internal.Data.Events;

namespace TextAdventures.Engine.Internal.Data.Projection
{
    public sealed class ProjectionManager : DomainEventSubscriber, ISubscribeTo<Room, RoomId, RoomCreatedEvent>
    {

    }
}