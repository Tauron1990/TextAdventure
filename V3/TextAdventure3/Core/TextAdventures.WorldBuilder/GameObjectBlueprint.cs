﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace TextAdventures.Builder
{
    [PublicAPI]
    public sealed class GameObjectBlueprint
    {
        public string Name { get; }

        public List<ComponentBlueprint> ComponentBlueprints { get; } = new();

        public GameObjectBlueprint(string name) => Name = name;

        public ComponentBlueprint WithComponent<TType>()
            where TType : IComponent
        {
            var blue = new ComponentBlueprint(typeof(TType), this);
            ComponentBlueprints.Add(blue);
            return blue;
        }
    }
}