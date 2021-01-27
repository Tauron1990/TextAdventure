using System;
using Tauron.Features;

namespace TextAdventures.Engine.Systems
{
    public interface IConsumeEvent<TEvent, TState>
    {
        IObservable<TState> Process(IObservable<StatePair<TEvent, TState>> eventObservable);
    }
}