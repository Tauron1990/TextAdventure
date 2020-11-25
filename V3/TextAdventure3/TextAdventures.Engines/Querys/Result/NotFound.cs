namespace TextAdventures.Engine.Querys.Result
{
    public sealed record NotFound : QueryResult
    {
        public static readonly QueryResult Instance = new NotFound();
        public override        bool        Compled => false;
    }
}