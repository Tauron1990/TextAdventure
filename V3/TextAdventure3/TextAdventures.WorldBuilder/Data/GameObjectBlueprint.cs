using System.Collections.Immutable;
using Akka.Actor;

namespace TextAdventures.Builder.Data
{
    public sealed record GameObjectBlueprint(Props? CustomManager, ImmutableList<IComponentBlueprint> Components);
}