using System;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Tauron.Features;
using TextAdventures.Engine.Modules;

namespace TextAdventures.Builder
{
    [PublicAPI]
    public sealed class World
    {
        private Action<Exception>? _error;
        private ImmutableList<object> _gameMasterMessages;
        private ImmutableList<GameObjectBlueprint> _gameObjectBlueprints;
        private ImmutableDictionary<string, IPreparedFeature> _gameProcesses;

        public World()
        {
            _gameObjectBlueprints = ImmutableList<GameObjectBlueprint>.Empty;
            _gameProcesses = ImmutableDictionary<string, IPreparedFeature>.Empty;
            _gameMasterMessages = ImmutableList<object>.Empty;
            Modules = ImmutableList<IModule>.Empty;
        }

        public ImmutableList<IModule> Modules { get; private set; }

        public GameSetup CreateSetup()
            => new(_gameObjectBlueprints, _gameMasterMessages, _gameProcesses, _error ?? (_ => { }), string.Empty,
                Modules);

        public World WithGlobalComponent(ComponentBlueprint blueprint)
            => WithGameMasterMessages(blueprint);

        public World WithModule(IModule module)
        {
            Modules = Modules.Add(module);
            return this;
        }

        public World WithGameMasterMessages(object msg)
        {
            _gameMasterMessages = _gameMasterMessages.Add(msg);
            return this;
        }

        public World WithErrorHandler(Action<Exception> handler)
        {
            _error = (Action<Exception>) Delegate.Combine(_error, handler);
            return this;
        }

        public World WithProcess(string name, IPreparedFeature process)
        {
            _gameProcesses = _gameProcesses.Add(name, process);
            return this;
        }

        public World WithGameObject(GameObjectBlueprint blueprint)
        {
            _gameObjectBlueprints = _gameObjectBlueprints.Add(blueprint);
            return this;
        }
    }
}