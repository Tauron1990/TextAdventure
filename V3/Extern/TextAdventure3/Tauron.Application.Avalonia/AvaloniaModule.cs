﻿using Autofac;
using JetBrains.Annotations;
using Tauron.Application.CommonUI;

namespace Tauron.Application.Avalonia
{
    [PublicAPI]
    public sealed class AvaloniaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AvaloniaFramework>().As<CommonUIFramework>();
            base.Load(builder);
        }
    }
}