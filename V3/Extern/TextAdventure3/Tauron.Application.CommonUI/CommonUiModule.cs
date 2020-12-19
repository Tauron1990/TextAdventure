using Autofac;
using JetBrains.Annotations;
using Tauron.Application.CommonUI.AppCore;
using Tauron.Host;

namespace Tauron.Application.CommonUI
{
    [PublicAPI]
    public sealed class CommonUiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppLifetime>().Named<IAppRoute>("default");
            
            base.Load(builder);
        }
    }
}