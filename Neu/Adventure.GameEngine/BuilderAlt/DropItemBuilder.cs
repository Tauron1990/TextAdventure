using System;
using System.Collections.Generic;
using Adventure.GameEngine.Builder.CommandData;
using Adventure.GameEngine.BuilderAlt.ItemData;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using Adventure.GameEngine.Core.Persistence;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.BuilderAlt
{
    [PublicAPI]
    public sealed class DropItemBuilder : ItemBuilder<DropItemBuilder>, IBluePrintProvider, IRoomItemBuilder<DropItemBuilder>
    {
        public const string DropItemPrefix = "DropItem_";

        private readonly RoomBuilder _builder;
        private readonly string _id;
        private readonly IInternalGameConfiguration _gameConfiguration;


        public DropItemBuilder(RoomBuilder builder, string id, IInternalGameConfiguration gameConfiguration)  
            : base(id)
        {
            builder.NewEntities.Add(this);
            BluePrint.CanDrop = true;

            BluePrint.Location = builder.Name;
            _builder = builder;
            _id = id;
            _gameConfiguration = gameConfiguration;
        }

        public DropItemBuilder(RoomBuilder builder, ItemBuilder baseBuilder, IInternalGameConfiguration gameConfiguration)
            : base(baseBuilder.BluePrint)
        {
            baseBuilder.BluePrint.CanDrop = true;
            BluePrint.Location = builder.Name;
            _id = baseBuilder.BluePrint.Id;
            _builder = builder;
            _gameConfiguration = gameConfiguration;
        }
        
        public DropItemBuilder PickUpCommand(Action<CommandModifaction<DropItemBuilder, PickupCommand>>? config, out CommandId id)
        {
            var command = new PickupCommand(_builder.Name, _id)
            {
                HideOnExecute = true
            };
            id = _gameConfiguration.RegisterCommand(command);
            _builder.Blueprints.Add(new RoomCommand(command, LazyString.New(GameConsts.PickUpCommand).AddParameters(_id)));
            config?.Invoke(new CommandModifaction<DropItemBuilder, PickupCommand>(command, this));
            return this;
        }

        public DropItemBuilder PickUpCommand(Action<CommandModifaction<DropItemBuilder, PickupCommand>>? config = null)
            => PickUpCommand(config, out _);

        IEnumerable<IBlueprint> IBluePrintProvider.Blueprints => new IBlueprint[]
        { 
            BluePrint, 
            new PersitBlueprint(DropItemPrefix + _id),
            new PointOfIntrestAddr(string.IsNullOrWhiteSpace(BluePrint.Description.Text) ? null : new PointOfInterst(true, BluePrint.Description)), 
        };

        void IBluePrintProvider.Validate()
        {
            if(string.IsNullOrWhiteSpace(BluePrint.Location))
                throw new InvalidOperationException("No Location for item");

            _gameConfiguration.RegisterItem(this);
        }

        string IRoomItemBuilder<DropItemBuilder>.Id => _id;

        RoomBuilder IRoomItemBuilder<DropItemBuilder>.Room => _builder;

        DropItemBuilder IRoomItemBuilder<DropItemBuilder>.Impl => this;
    }
}