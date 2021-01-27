using TextAdventures.Builder;

namespace TextAdventures.Engine.Modules.Text
{
    public sealed class TextModule : ModuleBase
    {
        public override void Enrich(World world)
        {
            world.WithProcess("Text_Module_Coordinator", TextCoordinator.Prefab())
                 .WithGlobalComponent(ComponentBlueprint.Single<TextLayerComponent>());
        }
    }
}