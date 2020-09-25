namespace TextAdventures.Engine.Querys.Result
{
    public sealed class NotFound : QueryResult
    {
        public override bool Compled => false;

        public static readonly QueryResult Instance = new NotFound();
    }
}