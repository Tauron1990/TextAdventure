using Adventure.GameEngine.Blueprints;
using Adventure.GameEngine.Core;
using Adventure.TextProcessing.Synonyms;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Rooms
{
    public class ItemBuilder
    {
        public DropItemBluePrint BluePrint { get; }

        public ItemBuilder(DropItemBluePrint bluePrint) => BluePrint = bluePrint;
    }

    public sealed class GenericItem : ItemBuilder<GenericItem>
    {
        public GenericItem(string id) 
            : base(new DropItemBluePrint(id))
        { }
    }

    [PublicAPI]
    public class ItemBuilder<TReturn> : ItemBuilder
        where TReturn : ItemBuilder<TReturn>
    {
        public ItemBuilder(DropItemBluePrint bluePrint) : base(bluePrint)
        {
        }

        public TReturn WithAction(VerbCodes verbCodes)
        {
            BluePrint.Action = verbCodes;
            return (TReturn) this;
        }

        public TReturn WithDescription(LazyString description)
        {
            BluePrint.Description = description;
            return (TReturn) this;
        }

        public TReturn WithDescription(string description)
        {
            BluePrint.Description = new LazyString(description);
            return (TReturn) this;
        }

        public TReturn WithEventTrigger(string? @event)
        {
            BluePrint.TriggerEvent = @event;
            return (TReturn) this;
        }

        public TReturn WithDisplayName(string name)
        {
            BluePrint.DisplayName = name;
            return (TReturn)this;
        }
    }
}