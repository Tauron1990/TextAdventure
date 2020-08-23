using Adventure.GameEngine.Core;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    public static class ChangeItem
    {
        public static ChangeItemCommand Description(string id, LazyString description)
            => new ChangeDescription(id, description);
    }

    public abstract class ChangeItemCommand : Command<ChangeItemCommand>
    {
        protected ChangeItemCommand(string id) 
            : base(id)
        {
        }

        protected internal abstract void Process(RoomItem item);
    }

    [PublicAPI]
    public sealed class ChangeDescription : ChangeItemCommand
    {
        public LazyString Description { get; }

        public ChangeDescription(string itemid, LazyString description) : base(itemid)
            => Description = description;

        protected internal override void Process(RoomItem item)
        {
            item.Data.Description = Description;
        }
    }
}