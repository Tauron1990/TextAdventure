using System;

namespace TextAdventures.Engine.Querys.Result
{
    public abstract class QueryResult
    {
        public abstract bool Compled { get; }

        public static QueryResult NotFound() => Result.NotFound.Instance;

        public static QueryResult Error(Exception e) => new QueryFailed(e);

        public static QueryResult Compleded(object result) => new QueryCompled(result);
    }
}
    