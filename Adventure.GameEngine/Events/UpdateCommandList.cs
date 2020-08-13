using System.Collections.Generic;

namespace Adventure.GameEngine.Events
{
    public sealed class UpdateCommandList
    {
        public IEnumerable<string> Commands { get; }

        public UpdateCommandList(IEnumerable<string> commands) => Commands = commands;
    }
}