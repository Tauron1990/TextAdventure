using System.Collections.Generic;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Blueprints;

namespace Adventure.GameEngine.Builder.Core
{
    public sealed class SimpleBlueprintProvider : IBluePrintProvider
    {
        private readonly IBlueprint[] _blueprints;

        public SimpleBlueprintProvider(params IBlueprint[] blueprints)
        {
            _blueprints = blueprints;
        }

        public IEnumerable<IBlueprint> Blueprints => _blueprints;

        public void Validate()
        {
        }
    }
}