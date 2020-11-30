using TextAdventures.Builder;

namespace TextAdventures.Engine.CommandSystem
{
    public static class Extensions
    {
        public static World WithCommandProcessor(this World world, RegisterCommandProcessorBase commandProcessor)
            => world.WithGameMasterMessages(commandProcessor);
    }
}