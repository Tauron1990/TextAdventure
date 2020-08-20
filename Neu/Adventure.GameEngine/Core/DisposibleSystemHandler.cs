using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Executor.Handlers;
using EcsRx.Systems;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core
{
    [UsedImplicitly, PublicAPI]
    public sealed class DisposibleSystemHandler : IConventionalSystemHandler
    {
        private readonly List<ISystem> _systems = new List<ISystem>();

        public void Dispose()
        {
            foreach (var disposable in _systems.Cast<IDisposable>()) 
                disposable.Dispose();

            _systems.Clear();
        }

        public bool CanHandleSystem(ISystem system) => system is IDisposable;

        public void SetupSystem(ISystem system) => _systems.Add(system);

        public void DestroySystem(ISystem system)
        {
            _systems.Remove(system);
            ((IDisposable)system).Dispose();
        }
    }
}