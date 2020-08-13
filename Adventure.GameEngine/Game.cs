using System;
using System.IO;
using System.Linq;
using Adventure.GameEngine.Blueprints;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
using Adventure.GameEngine.Internal;
using Adventure.GameEngine.Rooms;
using Adventure.Utilities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.ReactiveSystems;
using Newtonsoft.Json;

namespace Adventure.GameEngine
{
    public abstract class Game : EcsRx.Infrastructure.EcsRxApplication
    {
        private readonly string _saveGame;
        private readonly IStartUpNotify _notify;

        public event Action? OnStop;
        
        public override IDependencyContainer Container { get; }

        
        protected abstract int Version { get; }
        
        public ContentManagement Content { get; }
        
        protected Game(string saveGame, IStartUpNotify notify, ContentManagement content)
        {
            _saveGame = saveGame;
            _notify = notify;
            Content = content;

            Container = new NinjectDependencyContainer();
        }

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

        public override void StopApplication()
        {
            OnStop?.Invoke();
            SaveEntityDatabase();
            base.StopApplication();
        }

        protected override void ApplicationStarted()
        {
            try
            {
                if (File.Exists(_saveGame))
                {
                    LoadEntityDatabase();

                    var info = EntityDatabase.GetEntitiesFor(new Group(typeof(GameInfo))).Single().GetComponent<GameInfo>();
                    if(info.Version != Version)
                        throw new InvalidOperationException("Invalid Versions");

                    var roomConfig = new RoomConfiguration();
                    ConfigurateRooms(roomConfig);
                    roomConfig.Validate();

                    var entities = EntityDatabase.GetEntitiesFor(new Group(typeof(Room), typeof(RoomData)))
                        .ToDictionary(e => e.GetComponent<Room>().Name);

                    foreach (var room in roomConfig.Rooms)
                    {
                        if (entities.TryGetValue(room.Name, out var entity)) 
                            entity.ApplyBlueprints(room.Blueprints.OfType<RoomCommandSetup>());
                    }

                    EventSystem.Publish(new MapBuild());
                }
                else
                {
                    EntityDatabase.GetCollection().CreateEntity(new BaseGameInfo(Version));

                    var roomConfiguration = new RoomConfiguration();
                    var start = ConfigurateRooms(roomConfiguration);
                    roomConfiguration.Validate();
                    start.WithBluePrint(new StartRoom());

                    var map = EntityDatabase.GetCollection();
                    foreach (var room in roomConfiguration.Rooms)
                    {
                        room.WithBluePrint(new DoorWayConfiguration(room.DoorWays, room.Connections));

                        map.CreateEntity(room.Blueprints);
                    }

                    EntityDatabase.GetCollection().CreateEntity(new PlayerSetup(start.Name));

                    EventSystem.Publish(new MapBuild());
                }
            }
            catch (Exception e)
            {
                _notify.Fail(e);
            }

            _notify.Succed(this);
        }

        protected abstract RoomBuilder ConfigurateRooms(RoomConfiguration configuration);

        private void SaveEntityDatabase()
        {
            var save = new SaveStade();

            foreach (var entityDatabaseCollection in EntityDatabase.Collections)
            {
                var collData = new EntityCollectionData(entityDatabaseCollection.Id);
                foreach (var ent in entityDatabaseCollection)
                {
                    var entData = new EntityData();
                    entData.Components.AddRange(ent.Components.Where(c => !(c is RoomCommands)));
                    collData.Entitys.Add(entData);
                }
                save.Collections.Add(collData);
            }

            if (!Directory.Exists(Path.GetDirectoryName(_saveGame)))
                Directory.CreateDirectory(Path.GetDirectoryName(_saveGame));

            File.WriteAllText(_saveGame, JsonConvert.SerializeObject(save, Formatting.Indented));
        }

        private void LoadEntityDatabase()
        {
            var saveDate = JsonConvert.DeserializeObject<SaveStade>(File.ReadAllText(_saveGame));

            foreach (var collection in saveDate.Collections)
            {

                var coll = collection.Id == 0 ? EntityDatabase.GetCollection(collection.Id) : EntityDatabase.CreateCollection(collection.Id);

                foreach (var entity in collection.Entitys)
                {
                    var ent = coll.CreateEntity();
                    ent.AddComponents(entity.Components);
                }
            }
        }
    }
}