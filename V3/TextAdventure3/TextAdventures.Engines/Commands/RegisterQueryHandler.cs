using System;
using TextAdventures.Engine.Internal.Querys;
using TextAdventures.Engine.Querys;

namespace TextAdventures.Engine.Commands
{
    public sealed class RegisterQueryHandler<TQuery> : INewQueryHandler
        where TQuery : IGameQuery
    {
        private readonly IQueryHandler _handler;

        IQueryHandler INewQueryHandler.Handler => _handler;

        Type INewQueryHandler.Target => typeof(TQuery);

        public RegisterQueryHandler(GameQueryHandler<TQuery> handler) 
            => _handler = handler;
    }
}