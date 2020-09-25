using System;
using TextAdventures.Engine.Querys;

namespace TextAdventures.Engine.Internal.Querys
{
    public interface INewProjector
    {
        IQueryHandler Handler { get; }

        Type Target { get; }

        Type Key { get; }

        Type Projector { get; }

        void Install(Delegate installer);
    }
}