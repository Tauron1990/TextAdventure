using BattlePlanPath;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Events
{
    [PublicAPI]
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