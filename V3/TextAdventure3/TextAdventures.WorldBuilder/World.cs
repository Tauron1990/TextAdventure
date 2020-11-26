using System;
using System.Collections.Immutable;
using Akka.Actor;
using JetBrains.Annotations;

namespace TextAdventures.Builder
{
    [PublicAPI]
    public sealed class World
    {
        private ImmutableList<GameObjectBlueprint> _gameObjectBlueprints;

        private ImmutableDictionary<string, Props> _gameProcesses;

        private Action<Exception>? _error; 

        public GameSetup CreateSetup() 
            => new(_gameObjectBlueprints, _gameProcesses, _error ?? (_ => {}));

        public World()
        {
            _gameObjectBlueprints = ImmutableList<GameObjectBlueprint>.Empty;
            _gameProcesses = ImmutableDictionary<string, Props>.Empty;
        }

        public World WithErrorHandler(Action<Exception> handler)
        {
            _error = (Action<Exception>) Delegate.Combine(_error, handler);
            return this;
        }

        public World WithProcess<TType>(string name, params object[] args)
            where TType : ActorBase
        {
            _gameProcesses = _gameProcesses.Add(name, Props.Create<TType>(args));
            return this;
        }

        public World WithGameObject(GameObjectBlueprint blueprint)
        {
            _gameObjectBlueprints = _gameObjectBlueprints.Add(blueprint);
            return this;
        }
    }
}