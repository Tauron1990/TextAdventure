using TextAdventures.Builder;

namespace TextAdventures.Engine.Modules.Text
{
    public sealed class TextModule : ModuleBase
    {
        public override void Enrich(World world)
        {
            world.WithProcess<TextCoordinator>("Text_Module_Coordinator")
                 .WithGlobalComponent(ComponentBlueprint.Single<TextLayerComponent>());
        }
    }
}