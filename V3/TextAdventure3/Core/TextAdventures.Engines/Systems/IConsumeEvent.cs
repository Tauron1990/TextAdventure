using System;
using Tauron.Features;

namespace TextAdventures.Engine.Systems
{
    public interface IConsumeEvent<TEvent, TState>
    {
        IDisposable Process(IObservable<StatePair<TEvent, TState>> eventObservable);
    }
}