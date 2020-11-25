namespace TextAdventures.Engine.Querys.Result
{
    public sealed record QueryCompled(object Result) : QueryResult
    {
        public override bool Compled => true;
    }
}