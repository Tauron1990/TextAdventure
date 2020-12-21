using Autofac;
using Tauron.Application.CommonUI;

namespace Tauron.Application.Wpf
{
    public sealed class WpfModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WpfFramework>().As<CommonUIFramework>();
            base.Load(builder);
        }
    }
}