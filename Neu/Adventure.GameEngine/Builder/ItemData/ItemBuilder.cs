using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.ItemData
{
    [PublicAPI]
    public class ItemBuilder
    {
        public ItemBluePrint BluePrint { get; }

        public ItemBuilder(ItemBluePrint bluePrint) => BluePrint = bluePrint;
    }

    [PublicAPI]
    public sealed class GenericItem : ItemBuilder<GenericItem>
    {
        public GenericItem(string id) 
            : base(new ItemBluePrint(id))
        { }
    }

    [PublicAPI]
    public class ItemBuilder<TReturn> : ItemBuilder
        where TReturn : ItemBuilder<TReturn>
    {
        public ItemBuilder(ItemBluePrint bluePrint) 
            : base(bluePrint)
        {
        }


        public virtual TReturn WithAction(string action)
        {
            BluePrint.Action = action;
            return (TReturn) this;
        }

        public virtual TReturn WithDescription(LazyString description)
        {
            BluePrint.Description = description;
            return (TReturn) this;
        }

        public virtual TReturn WithEventTrigger(string? @event)
        {
            BluePrint.TriggerEvent = @event;
            return (TReturn) this;
        }

        public virtual TReturn WithDisplayName(string name)
        {
            BluePrint.DisplayName = name;
            return (TReturn)this;
        }
    }
}