using System;
using JetBrains.Annotations;

namespace TextAdventures.Engine.Querys.Result
{
    [PublicAPI]
    public abstract record QueryResult
    {
        public abstract bool Compled { get; }

        public static QueryResult NotFound() => Result.NotFound.Instance;

        public static QueryResult Error(Exception e) => new QueryFailed(e);

        public static QueryResult Compleded(object result) => new QueryCompled(result);
    }
}