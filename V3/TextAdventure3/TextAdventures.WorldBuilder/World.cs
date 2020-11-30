using System;
using System.Collections.Immutable;
using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Engine.Modules;

namespace TextAdventures.Builder
{
    [PublicAPI]
    public sealed class World
    {
        private Action<Exception>? _error;
        private ImmutableList<GameObjectBlueprint> _gameObjectBlueprints;
        private ImmutableDictionary<string, Props> _gameProcesses;
        private ImmutableList<object> _gameMasterMessages;

        public World()
        {
            _gameObjectBlueprints = ImmutableList<GameObjectBlueprint>.Empty;
            _gameProcesses = ImmutableDictionary<string, Props>.Empty;
            _gameMasterMessages = ImmutableList<object>.Empty;
            Modules = ImmutableList<IModule>.Empty;
        }

        public ImmutableList<IModule> Modules { get; private set; }

        public GameSetup CreateSetup()
            => new(_gameObjectBlueprints, _gameMasterMessages, _gameProcesses, _error ?? (_ => { }), string.Empty, Modules);

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