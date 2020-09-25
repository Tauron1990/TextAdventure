using System;
using TextAdventures.Engine.Querys;

namespace TextAdventures.Engine.Internal.Querys
{
    public interface INewQueryHandler
    {
        public IQueryHandler Handler { get; }

        public Type Target { get; }
    }
}