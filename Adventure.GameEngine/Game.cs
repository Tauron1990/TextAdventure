using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.Plugins.Views;
using Ninject;

namespace Adventure.GameEngine
{
    public abstract class Game : EcsRx.Plugins.Persistence.EcsRxPersistedApplication
    {
        public override IDependencyContainer Container { get; }

        protected Game(IKernel kernel) => Container = new NinjectDependencyContainer(kernel);

        protected override void LoadPlugins()
        {
            RegisterPlugin(new ViewsPlugin());
            RegisterPlugin(new ReactiveSystemsPlugin());
            RegisterPlugin(new ComputedsPlugin());
            RegisterPlugin(new BatchPlugin());
            base.LoadPlugins();
        }
    }
}