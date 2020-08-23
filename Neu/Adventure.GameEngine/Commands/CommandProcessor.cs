using System;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Systems;
using Adventure.GameEngine.Systems.Events;
using EcsRx.Collections.Database;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    [PublicAPI]
    public abstract class CommandProcessor<TCommand> : MultiEventReactionSystem, IDisposable
        where TCommand : Command<TCommand>
    {
        public RoomItems RoomItems { get; }

        public CurrentRoom CurrentRoom { get; }

        public PlayerInventory Inventory { get; }

        public IContentManagement Content { get; }

        public IEntityDatabase Database { get; }

        protected CommandProcessor(Game game) 
            : base(game.EventSystem)
        {
            Content = game.Content;
            Database = game.EntityDatabase;
            CurrentRoom = new CurrentRoom(game.ObservableGroupManager);
            Inventory = new PlayerInventory(game.ObservableGroupManager);
            RoomItems = new RoomItems(game.ObservableGroupManager, CurrentRoom);
        }

        protected override void Init()
        {
            Receive<TCommand>(ProcessCommandinternal);
            InternalInit();
        }

        private void ProcessCommandinternal(TCommand obj)
        {
            if(obj.Hidden) return;
            if (obj.HideOnExecute)
            {
                obj.Hidden = true;
                UpdateCommandList();
            }

            ProcessCommand(obj);
            if(!string.IsNullOrWhiteSpace(obj.TriggersEvent))
                EventSystem.Publish(new TriggerEvent(obj.TriggersEvent));
        }

        protected virtual void InternalInit()
        {

        }

        protected abstract void ProcessCommand(TCommand command);

        protected virtual void UpdateCommandList()
            => EventSystem.Publish(new ForceUpdateCommandList());

        protected virtual void UpdateTextContent(LazyString? message)
            => EventSystem.Publish(new CommandContentUpdate(message));

        public virtual void Dispose()
        {
            RoomItems.Dispose();
            CurrentRoom.Dispose();
            Inventory.Dispose();
            Database.Dispose();
        }
    }
}