using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.BuilderAlt.ItemData
{
    [PublicAPI]
    public class ItemBuilder
    {
        public ItemBluePrint BluePrint { get; }

        public ItemBuilder(ItemBluePrint bluePrint)
        {
            BluePrint = new ItemBluePrint(bluePrint.Id)
            {
                Action = bluePrint.Action,
                CanDrop = bluePrint.CanDrop,
                Description = bluePrint.Description,
                DisplayName = bluePrint.DisplayName,
                Location = bluePrint.Location,
                TriggerEvent = bluePrint.TriggerEvent
            };
        }

        public ItemBuilder(string id)
            => BluePrint = new ItemBluePrint(id);
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

        public ItemBuilder(string id)
            : base(id)
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