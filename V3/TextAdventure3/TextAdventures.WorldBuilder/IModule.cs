using JetBrains.Annotations;
using TextAdventures.Builder;

namespace TextAdventures.Engine.Modules
{
    [PublicAPI]
    public interface IModule
    {
        void Enrich(World world);

        GameSetup Enrich(GameSetup setup);
    }
}