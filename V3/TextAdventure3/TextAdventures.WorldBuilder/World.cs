using System;
using System.Collections.Immutable;
using Akka.Actor;

namespace TextAdventures.Builder
{
    public sealed class World
    {
        private ImmutableList<GameObjectBlueprint> _gameObjectBlueprints;

        private ImmutableDictionary<string, Props> _gameProcesses;

        private Action<Exception>? _error; 

        public GameSetup CreateSetup()
        {
            return new(_gameObjectBlueprints, _gameProcesses, _error ?? (_ => {}));
        }

        public World()
        {
            _gameObjectBlueprints = ImmutableList<GameObjectBlueprint>.Empty;
            _gameProcesses = ImmutableDictionary<string, Props>.Empty;
        }
    }
}