using Adventure.GameEngine.Core;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    [PublicAPI]
    public sealed class ChangeDescription : ChangeItemCommand
    {
        public LazyString Description { get; }

        public ChangeDescription(string itemid, LazyString description) : base(itemid)
            => Description = description;

        protected internal override void Process(RoomItem item)
        {
            item.Data.Description.Value = Description;
        }
    }

    public abstract class ChangeItemCommand : Command<ChangeItemCommand>
    {
        protected ChangeItemCommand(string id)
            : base(id)
        {
        }

        protected internal abstract void Process(RoomItem item);
    }
}