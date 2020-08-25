using Adventure.GameEngine.Commands;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class ObjectChangeCommandProcessor : CommandProcessor<ObjectChangeCommand>
    {
        public ObjectChangeCommandProcessor(Game game) 
            : base(game)
        {
        }

        protected override void ProcessCommand(ObjectChangeCommand command)
            => command.Run();
    }
}