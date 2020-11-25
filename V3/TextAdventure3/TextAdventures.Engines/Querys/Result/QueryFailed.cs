using System;

namespace TextAdventures.Engine.Querys.Result
{
    public sealed record QueryFailed(Exception Error) : QueryResult
    {
        public override bool Compled => false;
    }
}