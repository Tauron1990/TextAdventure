using BattlePlanPath;

namespace Adventure.GameEngine.Events
{
    public sealed class RequestPath
    {
        public string To { get; }

        public string From { get; }

        public PathResult<string>? Path { get; set; }

        public RequestPath(string to, string @from)
        {
            To = to;
            From = @from;
        }

    }
}