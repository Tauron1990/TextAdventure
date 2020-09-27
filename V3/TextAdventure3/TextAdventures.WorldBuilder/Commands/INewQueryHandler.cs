using System;
using TextAdventures.Builder.Querys;

namespace TextAdventures.Builder.Commands
{
    public interface INewQueryHandler
    {
        public IQueryHandler Handler { get; }

        public Type Target { get; }
    }
}