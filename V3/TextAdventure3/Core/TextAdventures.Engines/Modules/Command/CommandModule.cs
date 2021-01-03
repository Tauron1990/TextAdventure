using TextAdventures.Builder;

namespace TextAdventures.Engine.Modules.Command
{
    public sealed class CommandModule : ModuleBase
    {
        public override void Enrich(World world)
        {
            world.WithGlobalComponent(ComponentBlueprint.Single<CommandLayerComponent>())
                 .WithProcess<CommandCoordinator>("Command_Module_Coordinator");
        }
    }
}