using System.Collections.Generic;
using EcsRx.Blueprints;

namespace Adventure.GameEngine.Core.Blueprints
{
    public interface IBluePrintProvider
    {
        IEnumerable<IBlueprint> Blueprints { get; }

        void Validate();
    }
}