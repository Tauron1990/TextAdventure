using System;
using System.Collections.Concurrent;
using Akkatecture.Core;
using LiquidProjections;
using TextAdventures.Builder.Data.Commands;
using TextAdventures.Builder.Data.Querys;
using TextAdventures.Engine.Projection.Base;
using TextAdventures.Engine.Querys;

namespace TextAdventures.Engine.Commands
{
    public sealed class NewProjection<TKey, TProjection, TQueryHandler, TQuery> : INewProjector
        where TQueryHandler : GameQueryHandler<TQuery, TKey, TProjection>
        where TKey : IIdentity
        where TProjection : class, IProjectorData<TKey>, new()
        where TQuery : IGameQuery
    {
        private readonly Action<EventMapBuilder<TProjection, TKey, ProjectionContext>> _builder;
        private readonly TQueryHandler                                                 _handler;
        private readonly Action<IDisposable>                                           _onSubscripe;
        private readonly string                                                        _tag;

        public NewProjection(TQueryHandler handler, string tag, Action<IDisposable> onSubscripe, Action<EventMapBuilder<TProjection, TKey, ProjectionContext>> builder)
        {
            _handler     = handler;
            _tag         = tag;
            _onSubscripe = onSubscripe;
            _builder     = builder;
        }

        IQueryHandler INewProjector.Handler => _handler;

        Type INewProjector.Target => typeof(TQueryHandler);

        Type INewProjector.Key => typeof(TKey);

        Type INewProjector.Projector => typeof(TProjection);

        string INewProjector.Tag => _tag;

        void INewProjector.Install(Delegate installer)
            => _onSubscripe(((Func<string, ConcurrentDictionary<TKey, TProjection>, Action<EventMapBuilder<TProjection, TKey, ProjectionContext>>, IDisposable>) installer)(_tag, _handler.Store, _builder));
    }

    public static class NewProjection
    {
        public static NewProjection<TKey, TProjection, TQueryHandler, TQuery> Create<TKey, TProjection, TQueryHandler, TQuery>(TQueryHandler handler, string tag, Action<IDisposable> onSubscripe, Action<EventMapBuilder<TProjection, TKey, ProjectionContext>> builder)
            where TKey : IIdentity
            where TProjection : class, IProjectorData<TKey>, new()
            where TQueryHandler : GameQueryHandler<TQuery, TKey, TProjection>
            where TQuery : IGameQuery => new(handler, tag, onSubscripe, builder);
    }
}