using Adventure.GameEngine.Core;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.CommandData
{
    [PublicAPI]
    public interface ICommandProperties
    {
        RoomItems RoomItems { get; }

        CurrentRoom CurrentRoom { get; }

        PlayerInventory Inventory { get; }

        DelegateCommand Command { get; }

        IEventSystem EventSystem { get; }

        IObservableGroupManager ObservableGroupManager { get; }

        IContentManagement Content { get; }

        IEntityDatabase Database { get; }
    }
}