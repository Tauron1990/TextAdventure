using System.Collections.Generic;
using JetBrains.Annotations;

namespace TextAdventures.Builder
{
    [PublicAPI]
    public sealed class GameObjectBlueprint
    {
        public string Name { get; }

        public List<ComponentBlueprint> ComponentBlueprints { get; } = new();
        
        public GameObjectBlueprint(string name) => Name = name;

        public GameObjectBlueprint WithComponent<TType>()
        {
            ComponentBlueprints.Add(new ComponentBlueprint(typeof(TType)));
            return this;
        }
    }
}