using JetBrains.Annotations;
using TextAdventures.Builder;
using TextAdventures.Engine.Modules.Command;
using TextAdventures.Engine.Modules.Text;

namespace TextAdventures.Engine.Modules
{
    [PublicAPI]
    public static class DefaultModules
    {
        public static void AddDefaultModules(World world)
        {
            world.WithModule(new TextModule())
                 .WithModule(new CommandModule());
        }
    }
}