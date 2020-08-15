using System;
using System.Collections.Generic;
using Adventure.GameEngine.Blueprints;
using EcsRx.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Rooms
{
    [PublicAPI]
    public sealed class DropItemBuilder : ItemBuilder<DropItemBuilder>, IBluePrintProvider
    {
        public const string DropItemPrefix = "DropItem_";

        private readonly RoomBuilder _builder;
        private readonly string _id;


        public DropItemBuilder(RoomBuilder builder, string id)  
            : base(new DropItemBluePrint(id))
        {
            builder.NewEntities.Add(this);

            BluePrint.Location = builder.Name;
            _builder = builder;
            _id = id;
        }

        public DropItemBuilder(RoomBuilder builder, ItemBuilder baseBuilder)
            : base(baseBuilder.BluePrint)
        {
            BluePrint.Location = builder.Name;
            _id = baseBuilder.BluePrint.Id;
            _builder = builder;
        }

        public RoomBuilder InRoom() 
            => _builder;

        IEnumerable<IBlueprint> IBluePrintProvider.Blueprints => new []{ BluePrint };

        void IBluePrintProvider.Validate()
        {
            if(string.IsNullOrWhiteSpace(BluePrint.Location))
                throw new InvalidOperationException("No Location for item");

            if(_builder.Root.CustomData.ContainsKey(DropItemPrefix + _id + BluePrint.Location))
                throw new InvalidOperationException("Duplicate Item");

            _builder.Root.CustomData[DropItemPrefix + _id + BluePrint.Location] = this;
        }
    }
}