namespace TextAdventures.Engine.Querys.Result
{
    public sealed class QueryCompled : QueryResult
    {
        public override bool Compled => true;

        public object Result { get; }

        public QueryCompled(object result) => Result = result;
    }
}