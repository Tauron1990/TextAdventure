﻿using Akka.Actor;
using Tauron.Localization.Extension;

namespace Tauron.Localization
{
    public sealed class LocExtensionId : ExtensionIdProvider<LocExtension>
    {
        public override LocExtension CreateExtension(ExtendedActorSystem system)
        {
            return new LocExtension().Init(system);
        }
    }
}