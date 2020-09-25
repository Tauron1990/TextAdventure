using System;
using System.Collections.Concurrent;
using Akkatecture.Core;
using LiquidProjections;
using TextAdventures.Engine.Internal.Data.Projection;
using TextAdventures.Engine.Internal.Querys;
using TextAdventures.Engine.Querys;

namespace TextAdventures.Engine.Commands
{
    public sealed class AddProjection<TKey, TProjection, TQueryHandler, TQuery> : INewProjector
        where TQueryHandler : GameQueryHandler<TQuery, TKey, TProjection>
        where TKey : IIdentity
        where TProjection : class, IProjectorData<TKey>, new()
        where TQuery : IGameQuery
    {
        private readonly TQueryHandler _handler;
        private readonly string _tag;
        private readonly Action<IDisposable> _onSubscripe;
        private readonly Action<EventMapBuilder<TProjection, TKey, ProjectionContext>> _builder;

        public AddProjection(TQueryHandler handler, string tag, Action<IDisposable> onSubscripe, Action<EventMapBuilder<TProjection, TKey, ProjectionContext>> builder)
        {
            _handler = handler;
            _tag = tag;
            _onSubscripe = onSubscripe;
            _builder = builder;
        }

        IQueryHandler INewProjector.Handler => _handler;

        Type INewProjector.Target => typeof(TQueryHandler);

        Type INewProjector.Key => typeof(TKey);

        Type INewProjector.Projector => typeof(TProjection);

        void INewProjector.Install(Delegate installer) 
            => _onSubscripe(((Func<string, ConcurrentDictionary<TKey, TProjection>, Action<EventMapBuilder<TProjection, TKey, ProjectionContext>>, IDisposable>) installer)
                (_tag, _handler.Store, _builder));
    }
}