using System;
using TextAdventures.Builder.Querys;

namespace TextAdventures.Builder.Commands
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