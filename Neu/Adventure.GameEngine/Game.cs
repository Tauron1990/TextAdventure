using System;
using System.IO;
using Adventure.GameEngine.Builder;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using Adventure.GameEngine.Core.Persistence;
using Adventure.GameEngine.Systems.Events;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.ReactiveSystems;
using JetBrains.Annotations;

namespace Adventure.GameEngine
{
    [PublicAPI]
    public abstract class Game : EcsRxApplication
    {
        private readonly IStartUpNotify _notify;
        private readonly string _saveGame;

        protected Game(string saveGame, IStartUpNotify notify, IContentManagement content)
        {
            _saveGame = saveGame;
            _notify = notify;
            Content = content;

            Container = new NinjectDependencyContainer();
        }

        public override IDependencyContainer Container { get; }

        protected Persister Persister { get; private set; } = null!;

        protected abstract int Version { get; }

        [PublicAPI]
        public IContentManagement Content { get; }

        public event Action? OnStop;

        protected override void LoadPlugins()
        {
            RegisterPlugin(new ReactiveSystemsPlugin());
            RegisterPlugin(new ComputedsPlugin());
            RegisterPlugin(new BatchPlugin());
            base.LoadPlugins();
        }

        protected override void BindSystems()
        {
            this.BindAllSystemsWithinApplicationScope();
            this.BindAllSystemsInAssemblies(typeof(Game).Assembly);
            this.BindAllSystemsInNamespaces("Adventure.Ui.Systems");
        }

        protected override void LoadModules()
        {
            Container.LoadModule(new GameModule(this));
            base.LoadModules();
        }

        public override void StopApplication()
        {
            OnStop?.Invoke();
            base.StopApplication();
            SaveEntityDatabase();
        }

        protected override void ApplicationStarted()
        {
            try
            {
                Persister = new Persister(EntityDatabase);
                LoadResiources();

                EntityDatabase.GetCollection().CreateEntity(new BaseGameInfo(Version));

                var roomConfiguration = new GameConfiguration(Container);
                var start = ConfigurateGame(roomConfiguration);
                roomConfiguration.Validate();
                start.WithBluePrint(new StartRoom());

                var entityCollection = EntityDatabase.GetCollection();
                foreach (var room in roomConfiguration.Rooms.Rooms)
                {
                    foreach (var entity in room.NewEntities)
                        entityCollection.CreateEntity(entity.Blueprints);

                    room.WithBluePrint(new DoorWayConfiguration(room.DoorWays, room.Connections));

                    entityCollection.CreateEntity(room.Blueprints);
                }

                EntityDatabase.GetCollection().CreateEntity(new PlayerSetup(start.Name));

                if (File.Exists(_saveGame))
                    LoadEntityDatabase();

                EventSystem.Publish(new GameBuild());
            }
            catch (Exception e)
            {
                _notify.Fail(e);
            }

            _notify.Succed(this);
        }

        protected virtual void LoadResiources()
        {
        }

        protected virtual IEntity LoadEntity(string name, IEntityCollection collection)
        {
            throw new InvalidOperationException("No Entity Found");
        }

        protected abstract RoomBuilder ConfigurateGame(GameConfiguration configuration);

        private void SaveEntityDatabase()
            => Persister.Save(_saveGame);

        private void LoadEntityDatabase()
            => Persister.Load(_saveGame, LoadEntity);
    }
}