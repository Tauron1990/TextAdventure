using System.Collections.Generic;
using EcsRx.Blueprints;

namespace Adventure.GameEngine.Rooms
{
    public interface IBluePrintProvider
    {
        IEnumerable<IBlueprint> Blueprints { get; }

        void Validate();
    }
}