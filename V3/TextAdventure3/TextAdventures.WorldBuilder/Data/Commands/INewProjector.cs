using System;
using TextAdventures.Builder.Data.Querys;

namespace TextAdventures.Builder.Data.Commands
{
    public interface INewProjector
    {
        IQueryHandler Handler { get; }

        Type Target { get; }

        Type Key { get; }

        Type Projector { get; }

        string Tag { get; }

        void Install(Delegate installer);
    }
}