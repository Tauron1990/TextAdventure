using System;
using System.IO;
using System.Linq;
using Adventure.GameEngine.Blueprints;
using Adventure.GameEngine.Components;
using Adventure.Utilities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.Plugins.Views;

namespace Adventure.GameEngine
{
    public abstract class Game : EcsRx.Plugins.Persistence.EcsRxPersistedApplication
    {
        private readonly IStartUpNotify _notify;

        public event Action? OnStop;
        
        public override IDependencyContainer Container { get; }

        public override string EntityDatabaseFile { get; }
        
        protected abstract int Version { get; }
        
        public ContentManagement Content { get; }

        public override bool LoadOnStart => false;

        protected Game(string saveGame, IStartUpNotify notify, ContentManagement content)
        {
            _notify = notify;
            EntityDatabaseFile = saveGame;
            Content = content;

            Container = new NinjectDependencyContainer();
        }

        protected override void LoadPlugins()
        {
            RegisterPlugin(new ViewsPlugin());
            RegisterPlugin(new ReactiveSystemsPlugin());
            RegisterPlugin(new ComputedsPlugin());
            RegisterPlugin(new BatchPlugin());
            base.LoadPlugins();
        }

        public override void StopApplication()
        {
            OnStop?.Invoke();
            base.StopApplication();
        }

        protected override void ApplicationStarted()
        {
            try
            {
                if (File.Exists(EntityDatabaseFile))
                {
                    var loader = LoadEntityDatabase();
                    if (!loader.IsCompletedSuccessfully)
                    {
                        _notify.Fail(new ArgumentException("Loading Failed"));
                        return;
                    }

                    var info = EntityDatabase.GetEntitiesFor(new Group(typeof(GameInfo))).Single().GetComponent<GameInfo>();
                    if(info.Version != Version)
                        throw new InvalidOperationException("Invalid Versions");
                }
                else
                {
                    EntityDatabase.GetCollection().CreateEntity(new BaseGameInfo(Version));
                }
            }
            catch (Exception e)
            {
                _notify.Fail(e);
            }
        }
    }
}