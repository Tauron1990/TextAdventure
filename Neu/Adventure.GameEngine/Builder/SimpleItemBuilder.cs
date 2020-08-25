using System;
using System.Collections.Generic;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.ItemData;
using Adventure.GameEngine.Core.Blueprints;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder
{
    [PublicAPI]
    public class SimpleItemBuilder<TEventSource> : ItemBuilder<TEventSource>, IBluePrintProvider, IRoomItemBuilder<SimpleItemBuilder<TEventSource>>, IHasRoot
        where TEventSource : IWithMetadata, IEntityConfiguration
    {
        public const string ItemPrefix = "Item_";

        private readonly RoomBuilder _builder;
        private readonly string _id;

        public SimpleItemBuilder(RoomBuilder builder, string id, Dictionary<string, object> metadata, TEventSource eventSource)
            : base(id, metadata, eventSource)
        {
            _builder = builder;
            _id = id;
            BluePrint.Location = builder.Name;

            AddBlueprint(BluePrint);
        }

        public SimpleItemBuilder(RoomBuilder builder, ItemBuilder baseBuilder, Dictionary<string, object> metadata, TEventSource eventSource)
            : base(baseBuilder.BluePrint, metadata, eventSource)
        {
            BluePrint.Location = builder.Name;
            _id = baseBuilder.BluePrint.Id;
            _builder = builder;
        }
        
        IEnumerable<IBlueprint> IBluePrintProvider.Blueprints => new IBlueprint[]
        {
            BluePrint,
            new PersitBlueprint(ItemPrefix + _id)
        };

        void IBluePrintProvider.Validate()
        {
            if (string.IsNullOrWhiteSpace(BluePrint.Location))
                throw new InvalidOperationException("No Location for item");

            ((IWithSubEntity)_builder).AddToSubSubEntity(ItemPrefix + _id, this);
            Metadata.Add(ItemPrefix + _id, this);
        }

        string IRoomItemBuilder<SimpleItemBuilder<TEventSource>>.Id => _id;

        SimpleItemBuilder<TEventSource> IRoomItemBuilder<SimpleItemBuilder<TEventSource>>.Impl => this;
        IEntityConfiguration IHasRoot.Root => ((IHasRoot) _builder).Root;
    }
}