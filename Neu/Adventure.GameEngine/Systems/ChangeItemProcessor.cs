using System.Linq;
using Adventure.GameEngine.Commands;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class ChangeItemProcessor : CommandProcessor<ChangeItemCommand>
    {
        public ChangeItemProcessor(Game game) 
            : base(game)
        {
        }

        protected override void ProcessCommand(ChangeItemCommand command)
        {
            var item = RoomItems.Value.FirstOrDefault(i => i.Data.Id == command.Id);
            if(item == null) return;

            command.Process(item);
        }
    }
}