using System.Collections.Generic;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.Events;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.ItemData
{
    [PublicAPI]
    public class ItemBuilder : EntityConfiguration
    {
        public ItemBluePrint BluePrint { get; }

        public ItemBuilder(ItemBluePrint bluePrint, Dictionary<string, object> metadata)
        {
            Metadata = metadata;
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

        public ItemBuilder(string id, Dictionary<string, object> metadata)
        {
            Metadata = metadata;
            BluePrint = new ItemBluePrint(id);
        }

        protected override Dictionary<string, object> Metadata { get; }
    }

    [PublicAPI]
    public class ItemBuilder<TEventSource> : ItemBuilder, IEventable<TEventSource, ItemBluePrint>
        where TEventSource : IWithMetadata, IEntityConfiguration
    {
        private readonly TEventSource _eventSource;

        public ItemBuilder(ItemBluePrint bluePrint, Dictionary<string, object> metadata, TEventSource eventSource) 
            : base(bluePrint, metadata)
        {
            _eventSource = eventSource;
        }

        public ItemBuilder(string id, Dictionary<string, object> metadata, TEventSource eventSource)
            : base(id, metadata)
        {
            _eventSource = eventSource;
        }

        internal TEventSource EventSource => _eventSource;

        TEventSource IEventable<TEventSource, ItemBluePrint>.EventSource => _eventSource;
        ItemBluePrint IEventable<TEventSource, ItemBluePrint>.EventData => BluePrint;
    }
}