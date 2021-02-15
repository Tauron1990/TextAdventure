using JetBrains.Annotations;
using TextAdventures.Builder;

namespace TextAdventures.Engine.Modules
{
    [PublicAPI]
    public abstract class ModuleBase : IModule
    {
        public virtual void Enrich(World world)
        {
        }

        public virtual GameSetup Enrich(GameSetup setup) => setup;
    }
}