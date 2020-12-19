using System;
using Autofac;
using JetBrains.Annotations;
using Tauron.Host;

namespace Tauron.Application.CommonUI.AppCore
{
    [PublicAPI]
    public sealed class WpfConfiguration
    {
        internal readonly ContainerBuilder ServiceCollection;

        public WpfConfiguration(ContainerBuilder serviceCollection) => ServiceCollection = serviceCollection;

        public WpfConfiguration WithAppFactory(Func<IUIApplication> factory)
        {
            ServiceCollection.Register(_ => new DelegateAppFactory(factory)).As<IAppFactory>().IfNotRegistered(typeof(IAppFactory));
            return this;
        }

        public WpfConfiguration WithRoute<TRoute>(string name)
            where TRoute : class, IAppRoute
        {
            ServiceCollection.RegisterType<TRoute>().Named<IAppRoute>(name);
            return this;
        }
    }
}