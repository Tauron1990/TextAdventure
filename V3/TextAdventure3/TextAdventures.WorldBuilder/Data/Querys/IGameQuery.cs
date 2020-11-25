using System;

namespace TextAdventures.Builder.Data.Querys
{
    public interface IGameQuery
    {
        public Type Target { get; }
    }

    public abstract class GameQuery<TTarget> : IGameQuery
        where TTarget : GameQuery<TTarget>
    {
        Type IGameQuery.Target => typeof(TTarget);
    }
}