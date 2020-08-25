using System;
using System.Collections.Generic;
using Adventure.GameEngine.BuilderAlt.ItemData;
using Adventure.GameEngine.Core.Blueprints;
using Adventure.GameEngine.Core.Persistence;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.BuilderAlt
{
    [PublicAPI]
    public sealed class SimpleItemBuilder : ItemBuilder<SimpleItemBuilder>, IBluePrintProvider, IRoomItemBuilder<SimpleItemBuilder>
    {
        public const string ItemPrefix = "Item_";

        private readonly RoomBuilder _builder;
        private readonly string _id;
        private readonly IInternalGameConfiguration _gameConfiguration;

        private PointOfInterst? _poi = null;

        public SimpleItemBuilder(RoomBuilder builder, string id, IInternalGameConfiguration gameConfiguration)
            : base(id)
        {
            builder.NewEntities.Add(this);

            _builder = builder;
            _id = id;
            BluePrint.Location = builder.Name;
            _gameConfiguration = gameConfiguration;
        }

        public SimpleItemBuilder(RoomBuilder builder, ItemBuilder baseBuilder, IInternalGameConfiguration gameConfiguration)
            : base(baseBuilder.BluePrint)
        {
            BluePrint.Location = builder.Name;
            _id = baseBuilder.BluePrint.Id;
            _builder = builder;
            _gameConfiguration = gameConfiguration;
        }

        public SimpleItemBuilder WithPoi(PointOfInterst interst)
        {
            _poi = interst;
            return this;
        }

        IEnumerable<IBlueprint> IBluePrintProvider.Blueprints => new IBlueprint[]
        {
            BluePrint,
            new PersitBlueprint(ItemPrefix + _id),
            new PointOfIntrestAddr(_poi != null || string.IsNullOrWhiteSpace(BluePrint.Description.Text) ? _poi : new PointOfInterst(true, BluePrint.Description)),kkl
        };

        void IBluePrintProvider.Validate()
        {
            if (string.IsNullOrWhiteSpace(BluePrint.Location))
                throw new InvalidOperationException("No Location for item");

            _gameConfiguration.RegisterItem(this);
        }

        string IRoomItemBuilder<SimpleItemBuilder>.Id => _id;

        RoomBuilder IRoomItemBuilder<SimpleItemBuilder>.Room => _builder;

        SimpleItemBuilder IRoomItemBuilder<SimpleItemBuilder>.Impl => this;
    }
}