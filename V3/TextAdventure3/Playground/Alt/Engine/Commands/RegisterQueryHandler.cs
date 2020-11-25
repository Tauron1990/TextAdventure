using System;
using TextAdventures.Builder.Data.Commands;
using TextAdventures.Builder.Data.Querys;
using TextAdventures.Engine.Querys;

namespace TextAdventures.Engine.Commands
{
    public sealed class RegisterQueryHandler<TQuery> : INewQueryHandler
        where TQuery : IGameQuery
    {
        private readonly IQueryHandler _handler;

        public RegisterQueryHandler(GameQueryHandler<TQuery> handler)
            => _handler = handler;

        IQueryHandler INewQueryHandler.Handler => _handler;

        Type INewQueryHandler.Target => typeof(TQuery);
    }

    public static class RegisterQueryHandler
    {
        public static RegisterQueryHandler<TQuery> New<TQuery>(GameQueryHandler<TQuery> handler)
            where TQuery : IGameQuery
            => new(handler);
    }
}