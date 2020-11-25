using System;
using TextAdventures.Builder.Data.Querys;

namespace TextAdventures.Builder.Data.Commands
{
    public interface INewQueryHandler
    {
        public IQueryHandler Handler { get; }

        public Type Target { get; }
    }
}