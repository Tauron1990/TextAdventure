using EcsRx.Collections;
using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.CommandData
{
    [PublicAPI]
    public interface ICommandProperties
    {
        DelegateCommand Command { get; }

        IEventSystem EventSystem { get; }

        IObservableGroupManager ObservableGroupManager { get; }
    }
}