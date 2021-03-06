﻿using System;
using System.IO;
using System.Linq;
using Adventure.GameEngine.Blueprints;
using Adventure.GameEngine.Builder;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Events;
using Adventure.GameEngine.Persistence;
using Adventure.TextProcessing;
using Adventure.Utilities.Interfaces;
using EcsRx.Extensions;
using EcsRx.Groups;
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

        protected Persister Persister { get; private set; }

        protected internal Parser Parser { get; } = new Parser();

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

                //if (File.Exists(_saveGame))
                //{
                //    LoadEntityDatabase();

                //    var info = EntityDatabase.GetEntitiesFor(new Group(typeof(GameInfo))).Single()
                //        .GetComponent<GameInfo>();
                //    if (info.Version != Version)
                //        throw new InvalidOperationException("Invalid Versions");

                //    var roomConfig = new RoomConfiguration(new CommonCommands(EventSystem, this), Parser);
                //    ConfigurateRooms(roomConfig);
                //    roomConfig.Validate();

                //    var entities = EntityDatabase.GetEntitiesFor(new Group(typeof(Room), typeof(RoomData)))
                //        .ToDictionary(e => e.GetComponent<Room>().Name);

                //    foreach (var room in roomConfig.Rooms)
                //        if (entities.TryGetValue(room.Name, out var entity))
                //            entity.ApplyBlueprints(room.Blueprints.OfType<RoomCommandSetup>());

                //    PostLoad();

                //    EventSystem.Publish(new MapBuild());
                //}
                //else
                //{
                    EntityDatabase.GetCollection().CreateEntity(new BaseGameInfo(Version));

                    var roomConfiguration = new RoomConfiguration(new CommonCommands(EventSystem, this), Parser);
                    var start = ConfigurateRooms(roomConfiguration);
                    roomConfiguration.Validate();
                    start.WithBluePrint(new StartRoom());

                    var entityCollection = EntityDatabase.GetCollection();
                    foreach (var room in roomConfiguration.Rooms)
                    {
                        foreach (var entity in room.NewEntities)
                            entityCollection.CreateEntity(entity.Blueprints);

                        room.WithBluePrint(new DoorWayConfiguration(room.DoorWays, room.Connections));

                        entityCollection.CreateEntity(room.Blueprints);
                    }

                    EntityDatabase.GetCollection().CreateEntity(new PlayerSetup(start.Name));

                    if(File.Exists(_saveGame))
                        LoadEntityDatabase();

                    EventSystem.Publish(new MapBuild());
                //}
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
        
        protected abstract RoomBuilder ConfigurateRooms(RoomConfiguration configuration);

        private void SaveEntityDatabase() 
            => Persister.Save(_saveGame);

        private void LoadEntityDatabase() 
            => Persister.Load(_saveGame, (s, collection) => throw new NotImplementedException("Currently not implemented"));
    }
}