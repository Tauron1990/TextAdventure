using System.Linq;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core.Querys;
using Adventure.GameEngine.Systems.Components;
using CodeProject.ObjectPool.Specialized;
using EcsRx.Extensions;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [UsedImplicitly]
    public sealed class LookProcessor : CommandProcessor<LookCommand>
    {
        private readonly IStringBuilderPool _pool;

        public LookProcessor(IStringBuilderPool pool, Game game) 
            : base(game)
            => _pool = pool;

        protected override void ProcessCommand(LookCommand command)
        {
            var roomData = CurrentRoom.Value.GetComponent<RoomData>();

            if (string.IsNullOrWhiteSpace(command.Target))
            {
                using var builder = _pool.GetObject();
                
                foreach (var interst in roomData.Pois)
                    builder.StringBuilder.AppendLine(interst.Text.Format(Content));

                UpdateTextContent(builder.ToString());
            }
            else
            {
                var ent = Database.GetCollection().Query(new QueryNamedItemFromRoom(command.Target, CurrentRoom.Name.Name)).FirstOrDefault();
                if(ent == null) return;

                UpdateTextContent(command.Responsd ?? ent.GetComponent<IngameObject>().Description.Value);
            }
        }
    }
}