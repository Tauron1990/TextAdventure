using System;

namespace TextAdventures.Engine.Querys.Result
{
    public sealed class QueryFailed : QueryResult
    {
        public override bool Compled => false;

        public Exception Error { get; }

        public QueryFailed(Exception error) => Error = error;
    }
}